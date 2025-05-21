using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    [SerializeField] private GameObject _fishPrefab;

    [Header("The amount of fish to spawn")]
    [Range(0,50000)]
    [SerializeField] private int _numberOfFish = 20;
    public GameObject[] AllFish;

    [Header("Fish boundery")]
    public Vector3 SwimLimits = new Vector3(5, 5, 5);
    public static FlockingManager Instance;

    [Header("Behaviour Settings")]
    [Range(0.1f, 5f)]
    public float MinSpeed;
    [Range(1f, 5f)]
    public float MaxSpeed;
    [Range(1f, 10f)]
    public float NeighbourDistance;
    [Range(1f, 5f)]
    public float RotationSpeed;

    [Header("Goal Location")]
    public Vector3 GoalPosition = Vector3.zero;
    [Range(0, 100)]
    [SerializeField] private int _goalRecalculateChance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        SpawnFish();
    }

    private void Update()
    {
        RecalculateGoalPosition();
    }

    private void SpawnFish()
    {
        AllFish = new GameObject[_numberOfFish];
        GoalPosition = this.transform.position;

        for (int i = 0; i < _numberOfFish; i++)
        {
            Vector3 spawnPosition = this.transform.position + new Vector3(Random.Range(-SwimLimits.x, SwimLimits.x),
                                                                     Random.Range(-SwimLimits.y, SwimLimits.y),
                                                                     Random.Range(-SwimLimits.z, SwimLimits.z));

            AllFish[i] = Instantiate(_fishPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void RecalculateGoalPosition()
    {
        if (Random.Range(0, 100) < _goalRecalculateChance)
        {
            GoalPosition = this.transform.position + new Vector3(Random.Range(-SwimLimits.x, SwimLimits.x),
                                                                     Random.Range(-SwimLimits.y, SwimLimits.y),
                                                                     Random.Range(-SwimLimits.z, SwimLimits.z));

        }
    }
    //private void OnDrawGizmos()
    //{
    //    Bounds fishTankBounds = new Bounds(this.transform.position, SwimLimits * 2);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(this.transform.position, fishTankBounds.size);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(GoalPosition, 1f);
    //}
}
