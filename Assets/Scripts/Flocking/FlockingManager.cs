using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using System.Collections.Generic;
using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
using System.Threading.Tasks;
using System;

public class FlockingManager : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;

    [Range(0,1f)]
    [SerializeField] private float _turnSpeed;
    [Range(0,10f)]
    [SerializeField] private float _moveSpeed;
    [Range(0, 100000)]
    [SerializeField] private int _maxPopulation;
    [Range(0,500)]
    [SerializeField] private int _numberOfNeighbors;
    [Range(0,10f)]
    [SerializeField] private float _schoolRadius;
    [Range(0, 1f)]
    [SerializeField] private float _agility;
    [Range(0, 10f)]
    [SerializeField] private float _alignmentWeight;
    [Range(0, 10f)]
    [SerializeField] private float _seperationWeight;
    [Range(0, 10f)]
    [SerializeField] private float _cohesionWeight;
    [Range(0, 360f)]
    [SerializeField] private float _visionAngle;
    [Range(0, 10f)]
    [SerializeField] private float _visionDistance;
    [SerializeField] private Transform _goal;
    [SerializeField] private Vector3 _swimLimits;
    [Range(0,0.5f)]
    [SerializeField] private float _tickDelay;

    private List<Matrix4x4> _fishTRS;
    private NativeList<Matrix4x4> _fishDataContainer;
    private JobHandle _handle;
    private Unity.Mathematics.Random _random;

    private void Awake()
    {
        _fishTRS = new List<Matrix4x4>();

        for (int i = 0; i < _maxPopulation; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                UnityEngine.Random.Range(-_swimLimits.x, _swimLimits.x),
                UnityEngine.Random.Range(-_swimLimits.y, _swimLimits.y),
                UnityEngine.Random.Range(-_swimLimits.z, _swimLimits.z));
            AddFish(spawnPosition, Quaternion.identity, 1);
        }
        _random = new Unity.Mathematics.Random(1122);
        _fishDataContainer = new NativeList<Matrix4x4>(1, Allocator.Persistent);
        StartCoroutine(Tick());
    }

    private void Update()
    {
        if (_fishTRS.Count > 0)
        {
            Graphics.DrawMeshInstanced(_mesh, 0, _material, _fishTRS);
        }
    }

    private void AddFish(Vector3 pos, Quaternion rot, float scale)
    {
        _fishTRS.Add(Matrix4x4.TRS(pos, rot, Vector3.one * scale));
    }

    IEnumerator Tick()
    {
        float lastTick = Time.time;
        while (true)
        {
            _fishDataContainer.SetCapacity(_fishTRS.Count);
            NativeArray<Matrix4x4> temp = new NativeArray<Matrix4x4>(_fishTRS.ToArray(), Allocator.TempJob);
            _fishDataContainer.CopyFrom(temp);
            temp.Dispose();
            yield return new WaitForFixedUpdate();

            UpdateBehaviourJob job = new UpdateBehaviourJob()
            {
                DeltaTime = Time.time - lastTick,
                MoveSpeed = _moveSpeed,
                FishDataContainer = _fishDataContainer,
                Random = _random,
                NumberOfNeighbors = _numberOfNeighbors,
                GoalPosition = _goal.position,
                SchoolRadius = _schoolRadius,
                SchoolUp = transform.up,
                Agility = _agility,
                AlignmentWeight = _alignmentWeight,
                CohesionWeight = _cohesionWeight,
                SeperationWeight = _seperationWeight,
                VisionAngle = _visionAngle,
                VisionDistance = _visionDistance,
            };

            lastTick = Time.time;
            _handle = job.Schedule(_fishTRS.Count, 8);
            yield return new WaitUntil(() => _handle.IsCompleted);
            _handle.Complete();

            Parallel.For(0, _fishTRS.Count, (i) =>
            {
                _fishTRS[i] = _fishDataContainer[i];
            });

            yield return new WaitForSeconds(_tickDelay);
        }
    }

    private void OnDestroy()
    {
        _handle.Complete();
        if (_fishDataContainer.IsCreated)
        {
            _fishDataContainer.Dispose();
        }
    }

    [BurstCompile]
    public struct UpdateBehaviourJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeList<Matrix4x4> FishDataContainer;

        [ReadOnly] public Unity.Mathematics.Random Random;
        [ReadOnly] public float MoveSpeed;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public int NumberOfNeighbors;
        [ReadOnly] public float3 GoalPosition;
        [ReadOnly] public float SchoolRadius;
        [ReadOnly] public float3 SchoolUp;
        [ReadOnly] public float Agility;
        [ReadOnly] public float AlignmentWeight;
        [ReadOnly] public float SeperationWeight;
        [ReadOnly] public float CohesionWeight;
        [ReadOnly] public float VisionAngle;
        [ReadOnly] public float VisionDistance;

        public void Execute(int index)
        {
            Matrix4x4 fishTRS = FishDataContainer[index];

            float3 position = fishTRS.GetPosition();
            Quaternion rotation = fishTRS.rotation;
            float3 scale = fishTRS.lossyScale;

            float3 fishForward = math.mul(rotation, new float3(0, 0, 1));
            float3 fishUp = math.mul(rotation, new float3(0, 1, 0));
            position += fishForward * MoveSpeed * DeltaTime;

            float3 separation = float3.zero;
            float3 alignment = float3.zero;
            float3 cohesion = float3.zero;
            float3 centerOfMass = float3.zero;
            float3 averageForward = float3.zero;
            int actualNeighbors = 0;

            int startIndex = Random.NextInt(0, FishDataContainer.Length);
            int endIndex = math.clamp(startIndex + NumberOfNeighbors, 0, FishDataContainer.Length);
            float cosVisionAngle = math.cos(math.radians(VisionAngle * 0.5f));
            float sqrVisionDist = VisionDistance * VisionDistance;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (i == index) continue;

                Matrix4x4 neighborTRS = FishDataContainer[i];
                float3 neighborPos = neighborTRS.GetPosition();
                float3 toNeighbor = neighborPos - position;
                float distSqr = math.lengthsq(toNeighbor);

                if (distSqr < 1e-4f || distSqr > sqrVisionDist) continue;

                float3 dirToNeighbor = math.normalize(toNeighbor);
                float dot = math.dot(fishForward, dirToNeighbor);
                if (dot < cosVisionAngle) continue;

                float distance = math.sqrt(distSqr);
                float3 neighborForward = math.mul(neighborTRS.rotation, new float3(0, 0, 1));
                averageForward += neighborForward;
                centerOfMass += neighborPos;

                float3 away = (position - neighborPos) / distance;
                separation += away * EaseAwayFromDirection(distance);

                actualNeighbors++;
            }

            float3 targetDirection;

            if (actualNeighbors > 0)
            {
                alignment = math.normalize(averageForward / actualNeighbors);
                float3 center = centerOfMass / actualNeighbors;
                cohesion = math.normalize(center - position);

                targetDirection = math.normalize(
                    separation * SeperationWeight +
                    alignment * AlignmentWeight +
                    cohesion * CohesionWeight
                );
            }
            else
            {
                // No neighbors seen — rotate toward goal
                float3 toGoal = GoalPosition - position;
                targetDirection = math.normalize(toGoal);
            }

            if (math.lengthsq(targetDirection) < 0.01f)
                targetDirection = fishForward;

            // Blend towards goal more the farther away the fish is
            float distanceFromGoal = math.length(GoalPosition - position);
            float goalWeight = math.saturate(distanceFromGoal / SchoolRadius);

            float3 goHomeDir = math.normalize(GoalPosition - position);
            float3 blendedDir = math.normalize(math.lerp(targetDirection, goHomeDir, goalWeight * 0.95f));

            Quaternion targetRotation = Quaternion.LookRotation(blendedDir, SchoolUp);
            rotation = Quaternion.Slerp(rotation, targetRotation, Agility);

            FishDataContainer[index] = Matrix4x4.TRS(position, rotation, scale);
        }

        private float EaseAwayFromDirection(float d)
        {
            return 1 / (1 + d);
        }
    }
}







