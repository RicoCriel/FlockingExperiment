using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    [Header("Fish spawning settings")]
    [SerializeField] private GameObject _fishPrefab;
    private GameObject[] _allFishObjects;

    [Range(0, 10000)]
    [SerializeField] private int _numberOfFish;
    private Flock[] _flockMembers;

    [Header("Fish boundary")]
    [SerializeField] private Vector3 _swimLimits;
    public Vector3 SwimLimits => _swimLimits;
    public float NeighbourDistanceSqr => NeighbourDistance * NeighbourDistance;

    [Header("Behaviour Settings")]
    [Range(0.1f, 5f)] public float MinSpeed;
    [Range(1f, 5f)] public float MaxSpeed;
    [Range(1f, 10f)] public float NeighbourDistance;
    [Range(1f, 5f)] public float RotationSpeed;
    [Range(1f, 360f)] public float ViewingAngle;
    [Range(1f, 10f)] public float SeparationWeight;
    [Range(1f, 10f)] public float CohesionWeight;
    [Range(1f, 10f)] public float AlignmentWeight;

    [Header("Spatial Partitioning")]
    private Octree _octree;
    private Dictionary<GameObject, Flock> _fishToFlockMap = new();

    public bool TryGetFlock(GameObject fish, out Flock flock) =>
        _fishToFlockMap.TryGetValue(fish, out flock);

    private Coroutine _batchSpawningRoutine;

    private void Start()
    {
        Bounds tankBounds = new(transform.position, _swimLimits * 2);
        _octree = new Octree(tankBounds);
        _batchSpawningRoutine = StartCoroutine(SpawnFishCoroutine());
    }

    private void Update()
    {
        // Update octree only for fish that moved significantly
        foreach (var fish in _allFishObjects)
        {
            if (fish != null)
            {
                var flock = fish.GetComponent<Flock>();
                _octree.UpdatePosition(fish, flock.LastPosition);
                flock.UpdateLastPosition();
            }
        }

        // Update fish behavior
        foreach (var fish in _flockMembers)
        {
            fish?.UpdateBehaviour(this, _octree);
        }
    }

    private IEnumerator SpawnFishCoroutine()
    {
        _allFishObjects = new GameObject[_numberOfFish];
        _flockMembers = new Flock[_numberOfFish];
        _fishToFlockMap.Clear();

        int batchSize = 100; // Spawn in batches to avoid freezing
        List<GameObject> currentBatch = new();

        for (int i = 0; i < _numberOfFish; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-_swimLimits.x, _swimLimits.x),
                Random.Range(-_swimLimits.y, _swimLimits.y),
                Random.Range(-_swimLimits.z, _swimLimits.z));

            var fish = Instantiate(_fishPrefab, spawnPosition, Quaternion.identity);
            _allFishObjects[i] = fish;

            if (fish.TryGetComponent<Flock>(out var flockMember))
            {
                _flockMembers[i] = flockMember;
                flockMember.CreateBoundary(this);
                flockMember.Initialize(this);
                _fishToFlockMap[fish] = flockMember;
            }

            currentBatch.Add(fish);

            // Batch insert every 100 fish
            if (currentBatch.Count >= batchSize || i == _numberOfFish - 1)
            {
                _octree.BatchInsert(currentBatch);
                currentBatch.Clear();
                yield return null; // Spread workload across frames
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_octree != null) DrawOctree(_octree._root);
    }

    private void DrawOctree(OctreeNode node)
    {
        if (node == null) return;

        Gizmos.color = Color.Lerp(Color.green, Color.red, node.Objects.Count / 10f);
        Gizmos.DrawWireCube(node.Bounds.center, node.Bounds.size);

        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                DrawOctree(child);
            }
        }
    }
}
