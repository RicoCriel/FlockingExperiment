using Unity.VisualScripting;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    [Header("Fish spawning settings")]
    [SerializeField] private GameObject _fishPrefab;
    private GameObject[] _allFishObjects;

    [Range(0,50000)]
    [SerializeField] private int _numberOfFish;
    private Flock[] _flockMembers;
    public Flock[] AllFish => _flockMembers;

    [Header("Fish boundery")]
    [SerializeField] private Vector3 _swimLimits;
    public Vector3 SwimLimits { get { return _swimLimits; } }

    [Header("Behaviour Settings")]
    [Range(0.1f, 5f)]
    public float MinSpeed;
    [Range(1f, 5f)]
    public float MaxSpeed;
    [Range(1f, 10f)]
    public float NeighbourDistance;
    [Range(1f, 5f)]
    public float RotationSpeed;
    [Range(1f, 180f)]
    [SerializeField] private float _viewingAngle;
    [Range(1f, 10f)]
    [SerializeField] private float _separationWeight;
    [Range(1f, 10f)]
    [SerializeField] private float _cohesionWeight;
    [Range(1f, 10f)]
    [SerializeField] private float _alignmentWeight;

    public float ViewingAngle { get { return _viewingAngle; } }
    public float SeparationWeight { get { return _separationWeight; } }
    public float CohesionWeight { get { return _cohesionWeight; } }
    public float AlignmentWeight { get { return _alignmentWeight; } }

    [Header("Goal Location")]
    public Vector3 GoalPosition = Vector3.zero;

    private Octree _octree;

    private void Start()
    {
        Bounds tankBounds = new Bounds(this.transform.position, _swimLimits * 2);
        _octree = new Octree(tankBounds);
        SpawnFish();
    }

    private void Update()
    {
        // Update fish positions in the octree
        foreach (var fish in _allFishObjects)
        {
            _octree.UpdatePosition(fish);
        }

        // Pass the octree to each fish
        foreach (var fish in _flockMembers)
        {
            fish.UpdateBehaviour(this, _octree); 
        }
    }

    private void SpawnFish()
    {
        if (_fishPrefab.GetComponent<Flock>() == null)
        {
            Debug.LogError("Fish prefab is missing the Flock component!");
            return;
        }

        _allFishObjects = new GameObject[_numberOfFish];
        _flockMembers = new Flock[_numberOfFish];

        GoalPosition = this.transform.position;

        for (int i = 0; i < _numberOfFish; i++)
        {
            Vector3 spawnPosition = this.transform.position + new Vector3(Random.Range(-SwimLimits.x, SwimLimits.x),
                                                                     Random.Range(-SwimLimits.y, SwimLimits.y),
                                                                     Random.Range(-SwimLimits.z, SwimLimits.z));

            _allFishObjects[i] = Instantiate(_fishPrefab, spawnPosition, Quaternion.identity);
            if (_allFishObjects[i].TryGetComponent<Flock>(out Flock flockMember))
            {
                _flockMembers[i] = flockMember; 
                flockMember.CreateBoundary(this);
                flockMember.Initialize(this);
            }
        }

        // Insert new fish into the octree
        foreach (var fishObj in _allFishObjects)
        {
            _octree.Insert(fishObj);
        }
    }

    private void OnDrawGizmos()
    {
        Bounds fishTankBounds = new Bounds(this.transform.position, SwimLimits * 2);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, fishTankBounds.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GoalPosition, 1f);
    }
}
