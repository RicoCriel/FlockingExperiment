using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public struct Boid
    {
        public Vector3 Position;
        public Vector3 Direction;

        public Boid(Vector3 pos, Vector3 dir)
        {
            Position = pos;
            Direction = dir;
        }
    }

    [Header("Boid Settings")]
    public int BoidsCount = 1000;
    public Mesh BoidMesh;
    public Material BoidMaterial;
    public ComputeShader shader;
    public Transform Target;

    [Header("Flocking Behavior")]
    public float NeighbourDistance = 1.5f;
    public float SeparationWeight = 1.2f;
    public float CohesionWeight = 1.0f;
    public float AlignmentWeight = 1.0f;
    public float BoidSpeedVariation = 1.0f;
    public float ViewingAngle = 120f;

    [Header("Speed Settings")]
    [Min(0)] public float MinSpeed = 1f;
    [Min(0)] public float MaxSpeed = 3f;
    [Min(0)] public float MinRotationSpeed = 0.5f;
    [Min(0)] public float MaxRotationSpeed = 2f;

    [Header("Tank Bounds")]
    [SerializeField] private Vector3 _swimLimits = new(10, 10, 10);
    public Vector3 SwimLimits => _swimLimits;

    [Header("Fish-like Tuning")]
    [Range(0.1f, 1f)] public float VerticalDamping = 0.7f;
    [Range(0f, 0.3f)] public float NoiseStrength = 0.15f;

    private ComputeBuffer _boidsBuffer;
    private GraphicsBuffer _argsBuffer;
    private Boid[] _boidsArray;

    private int _kernelHandle;
    private int _threadGroupSize;
    private int _numOfBoids;
    private RenderParams renderParams;

    void Start()
    {
        InitBoids();
        InitShader();
    }

    void InitBoids()
    {
        _kernelHandle = shader.FindKernel("CSMain");
        shader.GetKernelThreadGroupSizes(_kernelHandle, out uint threadX, out _, out _);
        _threadGroupSize = Mathf.CeilToInt((float)BoidsCount / threadX);
        _numOfBoids = _threadGroupSize * (int)threadX;

        _boidsArray = new Boid[_numOfBoids];

        for (int i = 0; i < _numOfBoids; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-_swimLimits.x, _swimLimits.x),
                Random.Range(-_swimLimits.y, _swimLimits.y),
                Random.Range(-_swimLimits.z, _swimLimits.z)
            );

            Vector3 dir = Random.insideUnitSphere;
            if (dir == Vector3.zero) dir = Vector3.forward;
            dir.Normalize();

            _boidsArray[i] = new Boid(pos, dir);
        }

        _boidsBuffer = new ComputeBuffer(_numOfBoids, sizeof(float) * 6);
        _boidsBuffer.SetData(_boidsArray);

        _argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        _argsBuffer.SetData(new GraphicsBuffer.IndirectDrawIndexedArgs[]
        {
            new GraphicsBuffer.IndirectDrawIndexedArgs
            {
                indexCountPerInstance = BoidMesh.GetIndexCount(0),
                instanceCount = (uint)_numOfBoids
            }
        });

        BoidMaterial.SetBuffer("boidsBuffer", _boidsBuffer);
        renderParams = new RenderParams(BoidMaterial)
        {
            worldBounds = new Bounds(Vector3.zero, Vector3.one * 1000)
        };
    }

    void InitShader()
    {
        shader.SetInt("boidCount", _numOfBoids);
        shader.SetFloat("neighbourDistance", NeighbourDistance);
        shader.SetFloat("separationWeight", SeparationWeight);
        shader.SetFloat("alignmentWeight", AlignmentWeight);
        shader.SetFloat("cohesionWeight", CohesionWeight);
        shader.SetFloat("viewingAngle", ViewingAngle);

        shader.SetFloat("minSpeed", MinSpeed);
        shader.SetFloat("maxSpeed", MaxSpeed);
        shader.SetFloat("minRotationSpeed", MinRotationSpeed);
        shader.SetFloat("maxRotationSpeed", MaxRotationSpeed);
        shader.SetFloat("rotationSpeed", Mathf.Min(MinRotationSpeed,MaxRotationSpeed));
        shader.SetFloat("boidSpeedVariation", BoidSpeedVariation);

        shader.SetFloat("verticalDamping", VerticalDamping);
        shader.SetFloat("noiseStrength", NoiseStrength);

        shader.SetVector("tankExtents", _swimLimits);
        shader.SetVector("tankCenter", transform.position);

        if (Target)
            shader.SetVector("flockPosition", Target.position);

        shader.SetBuffer(_kernelHandle, "boidsBuffer", _boidsBuffer);
    }

    void Update()
    {
        if (!shader || _boidsBuffer == null || _argsBuffer == null) return;

        shader.SetFloat("time", Time.time * 0.3f);
        shader.SetFloat("deltaTime", Time.deltaTime);

        if (Target)
            shader.SetVector("flockPosition", Target.position);

        shader.Dispatch(_kernelHandle, _threadGroupSize, 1, 1);
        Graphics.RenderMeshIndirect(renderParams, BoidMesh, _argsBuffer);
    }

    void OnDestroy()
    {
        _boidsBuffer?.Dispose();
        _argsBuffer?.Dispose();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, SwimLimits * 2f);
    }
}
