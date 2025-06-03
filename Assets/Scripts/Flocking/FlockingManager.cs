using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using System.Collections.Generic;
using System.Collections;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public class FlockingManager : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;

    [Range(0, 1f)][SerializeField] private float _turnSpeed;
    [Range(0, 10f)][SerializeField] private float _moveSpeed;
    [Range(0, 100000)][SerializeField] private int _maxPopulation;
    [Range(0, 500)][SerializeField] private int _numberOfNeighbors;
    [Range(0, 10f)][SerializeField] private float _schoolRadius;
    [Range(0, 1f)][SerializeField] private float _agility;
    [Range(0, 10f)][SerializeField] private float _alignmentWeight;
    [Range(0, 10f)][SerializeField] private float _seperationWeight;
    [Range(0, 10f)][SerializeField] private float _cohesionWeight;
    [Range(0, 360f)][SerializeField] private float _visionAngle;
    [Range(0, 10f)][SerializeField] private float _visionDistance;
    [SerializeField] private Transform _goal;
    [Range(0, 1f)][SerializeField] private float _goalAttractionStrength;
    [SerializeField] private Vector3 _swimLimits;
    [SerializeField] private Vector3 _tankCenter;
    [Range(0, 0.5f)][SerializeField] private float _tickDelay;

    private List<Matrix4x4> _fishTRS;
    private NativeList<Matrix4x4> _fishDataContainer;
    private JobHandle _handle;
    private Unity.Mathematics.Random _random;

    private NativeArray<Matrix4x4> _previousTRS;
    private NativeArray<Matrix4x4> _interpolatedTRS;
    private Matrix4x4[] _renderArray;
    private float _lastTickTime;

    public float MaxPopulation { get { return _maxPopulation; } } 
    public float AlignmentWeight { get { return _alignmentWeight; } }
    public float CohesionWeight { get { return _cohesionWeight; } }
    public float SeparationWeight { get { return _seperationWeight; } }
    public float GoalAttractionStrength { get { return _goalAttractionStrength; } }
    public (float min, float max) FishCountRange => (0, 100000);
    public (float min, float max) AlignmentRange => (0, 10);
    public (float min, float max) CohesionRange => (0, 10);
    public (float min, float max) SeparationRange => (0, 10);
    public (float min, float max) DiverAttractionRange => (0, 1);

    private void Awake()
    {
        _fishTRS = new List<Matrix4x4>();

        // Initialize NativeArrays for efficient parallel processing
        _previousTRS = new NativeArray<Matrix4x4>(_maxPopulation, Allocator.Persistent);
        _interpolatedTRS = new NativeArray<Matrix4x4>(_maxPopulation, Allocator.Persistent);
        _renderArray = new Matrix4x4[_maxPopulation];

        for (int i = 0; i < _maxPopulation; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                UnityEngine.Random.Range(-_swimLimits.x, _swimLimits.x),
                UnityEngine.Random.Range(-_swimLimits.y, _swimLimits.y),
                UnityEngine.Random.Range(-_swimLimits.z, _swimLimits.z));

            var fishMatrix = Matrix4x4.TRS(spawnPosition, Quaternion.identity, Vector3.one);
            _fishTRS.Add(fishMatrix);
            _previousTRS[i] = fishMatrix;
            _interpolatedTRS[i] = fishMatrix;
        }

        _random = new Unity.Mathematics.Random(1122);
        _fishDataContainer = new NativeList<Matrix4x4>(1, Allocator.Persistent);
        StartCoroutine(Tick());
    }

    private void Update()
    {
        if (_fishTRS.Count == 0) return;

        // Calculate interpolation factor based on time since last tick
        float t = Mathf.Clamp01((Time.time - _lastTickTime) / _tickDelay);

        // Create temporary NativeArray for current fish states
        using (NativeArray<Matrix4x4> currentFishData = new NativeArray<Matrix4x4>(_fishTRS.ToArray(), Allocator.TempJob))
        {
            // Schedule parallel interpolation job
            var interpolationJob = new InterpolationJob
            {
                Previous = _previousTRS,
                Current = currentFishData,
                Interpolated = _interpolatedTRS,
                T = t
            };

            JobHandle handle = interpolationJob.Schedule(_fishTRS.Count, 64);
            handle.Complete();
        }

        // Copy interpolated data to managd array for rendering
        _interpolatedTRS.CopyTo(_renderArray);
        Graphics.DrawMeshInstanced(_mesh, 0, _material, _renderArray, _fishTRS.Count);
    }

    IEnumerator Tick()
    {
        float nextTickTime = Time.time;
        while (true)
        {
            // Capture previous state before simulation
            for (int i = 0; i < _fishTRS.Count; i++)
            {
                _previousTRS[i] = _fishTRS[i];
            }

            // Copy data to NativeArray for job processing
            _fishDataContainer.SetCapacity(_fishTRS.Count);
            using (NativeArray<Matrix4x4> temp = new NativeArray<Matrix4x4>(_fishTRS.ToArray(), Allocator.TempJob))
            {
                _fishDataContainer.CopyFrom(temp);
            }

            // Configure and run the simulation job
            UpdateBehaviourJob job = new UpdateBehaviourJob()
            {
                DeltaTime = _tickDelay,
                MoveSpeed = _moveSpeed,
                FishDataContainer = _fishDataContainer,
                Random = _random,
                NumberOfNeighbors = _numberOfNeighbors,
                GoalPosition = _goal.position,
                GoalAttractionStrength = _goalAttractionStrength,
                SchoolRadius = _schoolRadius,
                SchoolUp = transform.up,
                Agility = _agility,
                AlignmentWeight = _alignmentWeight,
                CohesionWeight = _cohesionWeight,
                SeperationWeight = _seperationWeight,
                VisionAngle = _visionAngle,
                VisionDistance = _visionDistance,
                SwimLimits = _swimLimits,
                TankCenter = _tankCenter
            };

            _handle = job.Schedule(_fishTRS.Count, 64);
            yield return new WaitUntil(() => _handle.IsCompleted);
            _handle.Complete();

            // Update fish data from job results
            for (int i = 0; i < _fishTRS.Count; i++)
            {
                _fishTRS[i] = _fishDataContainer[i];
            }

            _lastTickTime = Time.time;

            // Wait for next tick
            nextTickTime += _tickDelay;
            float waitTime = nextTickTime - Time.time;
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);
            else
                yield return null; // Skip frame if behind schedule
        }
    }

    public void SetFishCount(float value)
    {
        _maxPopulation = Mathf.RoundToInt(value);
    }

    public void SetAlignmentWeight(float value) => _alignmentWeight = value;
    public void SetCohesionWeight(float value) => _cohesionWeight = value;
    public void SetSeparationWeight(float value) => _seperationWeight = value;
    public void SetDiverAttraction(float value) => _goalAttractionStrength = value;

    private void OnDestroy()
    {
        // Complete any running jobs
        _handle.Complete();

        // Clean up native collections
        if (_fishDataContainer.IsCreated)
            _fishDataContainer.Dispose();
        if (_previousTRS.IsCreated)
            _previousTRS.Dispose();
        if (_interpolatedTRS.IsCreated)
            _interpolatedTRS.Dispose();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(_tankCenter, _swimLimits * 2);
    //}

    [BurstCompile]
    public struct UpdateBehaviourJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeList<Matrix4x4> FishDataContainer;

        [ReadOnly] public Unity.Mathematics.Random Random;
        [ReadOnly] public float MoveSpeed;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public int NumberOfNeighbors;
        [ReadOnly] public float3 GoalPosition;
        [ReadOnly] public float GoalAttractionStrength;
        [ReadOnly] public float SchoolRadius;
        [ReadOnly] public float3 SchoolUp;
        [ReadOnly] public float Agility;
        [ReadOnly] public float AlignmentWeight;
        [ReadOnly] public float SeperationWeight;
        [ReadOnly] public float CohesionWeight;
        [ReadOnly] public float VisionAngle;
        [ReadOnly] public float VisionDistance;
        [ReadOnly] public float3 SwimLimits;
        [ReadOnly] public float3 TankCenter;

        public void Execute(int index)
        {
            Matrix4x4 fishTRS = FishDataContainer[index];

            float3 position = fishTRS.GetPosition();
            Quaternion rotation = fishTRS.rotation;
            float3 scale = fishTRS.lossyScale;

            float3 fishForward = math.mul(rotation, new float3(0, 0, 1));
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
                float3 toGoal = GoalPosition - position;
                targetDirection = math.normalize(toGoal);
            }

            float distanceFromGoal = math.length(GoalPosition - position);
            float goalWeight = math.saturate(distanceFromGoal / SchoolRadius);
            float3 goHomeDir = math.normalize(GoalPosition - position);

            float goalBlendFactor = math.saturate(goalWeight * GoalAttractionStrength);
            float3 blendedDir = math.normalize(math.lerp(
                targetDirection,
                goHomeDir,
                goalBlendFactor
            ));

            float3 localPos = position - TankCenter;
            float3 boundaryProximity = math.abs(localPos) / SwimLimits;

            if (math.any(boundaryProximity > 0.8f))
            {
                float3 toCenter = math.normalize(TankCenter - position);
                float3 boundaryWeight = math.saturate((boundaryProximity - 0.8f) * 5f);
                float avoidanceStrength = math.cmax(boundaryWeight);

                blendedDir = math.lerp(blendedDir, toCenter, avoidanceStrength * 0.95f);
                blendedDir = math.normalize(blendedDir);
            }

            Quaternion targetRotation = Quaternion.LookRotation(blendedDir, SchoolUp);
            rotation = Quaternion.Slerp(rotation, targetRotation, Agility);

            FishDataContainer[index] = Matrix4x4.TRS(position, rotation, scale);
        }

        [BurstCompile]
        private float EaseAwayFromDirection(float d)
        {
            return 1 / (1 + d);
        }
    }

    [BurstCompile]
    public struct InterpolationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Matrix4x4> Previous;
        [ReadOnly] public NativeArray<Matrix4x4> Current;
        [WriteOnly] public NativeArray<Matrix4x4> Interpolated;
        public float T;

        public void Execute(int index)
        {
            Matrix4x4 prev = Previous[index];
            Matrix4x4 curr = Current[index];

            // Extract position and rotation
            Vector3 prevPos = prev.GetPosition();
            Vector3 currPos = curr.GetPosition();
            Quaternion prevRot = prev.rotation;
            Quaternion currRot = curr.rotation;

            // Interpolate
            Vector3 pos = Vector3.Lerp(prevPos, currPos, T);
            Quaternion rot = Quaternion.Slerp(prevRot, currRot, T);

            // Create new TRS matrix
            Interpolated[index] = Matrix4x4.TRS(pos, rot, Vector3.one);
        }
    }
}











