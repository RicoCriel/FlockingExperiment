using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    private float _movementSpeed;
    private Bounds _fishTankBounds;
    private Vector3 _lastPosition;

    // Reusable lists to avoid Garbage Collection
    private readonly List<GameObject> _cachedQueryResults = new List<GameObject>();
    private readonly List<Flock> _cachedNeighbors = new List<Flock>();

    public void CreateBoundary(FlockingManager manager)
    {
        _fishTankBounds = new Bounds(manager.transform.position, manager.SwimLimits * 2);
        _lastPosition = transform.position;
    }

    public void Initialize(FlockingManager manager)
    {
        _movementSpeed = Random.Range(manager.MinSpeed, manager.MaxSpeed);
    }

    public void UpdateBehaviour(FlockingManager manager, Octree octree)
    {
        HandleFishBoundery(manager);
        ApplyFlockingRules(manager, octree);
        RecalculateSpeed(manager);
        MoveFish();
    }

    private void MoveFish()
    {
        transform.Translate(0, 0, _movementSpeed * Time.deltaTime);
    }

    private void HandleFishBoundery(FlockingManager manager)
    {
        if (!_fishTankBounds.Contains(transform.position))
        {
            RotateTowardsCenter(manager);
        }
    }

    private void ApplyFlockingRules(FlockingManager manager, Octree octree)
    {
        if (manager == null || octree == null) return;

        // Get nearby objects from octree (reuses list to avoid Garbage collecting)
        _cachedQueryResults.Clear();
        octree.QueryNeighbors(transform.position, manager.NeighbourDistance, _cachedQueryResults);

        // Convert to Flock components (skip self)
        _cachedNeighbors.Clear();
        foreach (var obj in _cachedQueryResults)
        {
            if (obj != gameObject && manager.TryGetFlock(obj, out Flock flock))
            {
                _cachedNeighbors.Add(flock);
            }
        }

        // Calculate flocking vectors
        Vector3 separation = CalculateSeparation(_cachedNeighbors);
        Vector3 alignment = CalculateAlignment(_cachedNeighbors, manager);
        Vector3 cohesion = CalculateCohesion(_cachedNeighbors, manager);

        // Combine vectors and apply rotation
        Vector3 direction = separation * manager.SeparationWeight +
                            alignment * manager.AlignmentWeight +
                            cohesion * manager.CohesionWeight;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                manager.RotationSpeed * Time.deltaTime
            );
        }
    }

    private Vector3 CalculateSeparation(List<Flock> neighbors)
    {
        Vector3 separationVector = Vector3.zero;
        int avoidCount = 0;

        foreach (Flock neighbor in neighbors)
        {
            float sqrDist = (neighbor.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < 1.0f) 
            {
                separationVector += (transform.position - neighbor.transform.position);
                avoidCount++;
            }
        }

        if (avoidCount > 0)
        {
            separationVector /= avoidCount;
        }

        return separationVector;
    }

    private Vector3 CalculateAlignment(List<Flock> neighbors, FlockingManager manager)
    {
        Vector3 alignmentVector = Vector3.zero;
        int alignmentCount = 0;
        float minCos = Mathf.Cos(manager.ViewingAngle * 0.5f * Mathf.Deg2Rad);

        foreach (Flock neighbor in neighbors)
        {
            Vector3 toNeighbor = neighbor.transform.position - transform.position;
            float sqrDist = toNeighbor.sqrMagnitude;

            // Check if within field of view
            float cosAngle = Vector3.Dot(transform.forward, toNeighbor.normalized);
            if (cosAngle >= minCos && sqrDist <= manager.NeighbourDistanceSqr)
            {
                alignmentVector += neighbor.transform.forward;
                alignmentCount++;
            }
        }

        if (alignmentCount > 0)
        {
            alignmentVector /= alignmentCount;
        }

        return alignmentVector;
    }

    private Vector3 CalculateCohesion(List<Flock> neighbors, FlockingManager manager)
    {
        Vector3 center = Vector3.zero;
        int groupSize = 0;
        float minCos = Mathf.Cos(manager.ViewingAngle * 0.5f * Mathf.Deg2Rad);

        foreach (Flock neighbor in neighbors)
        {
            Vector3 toNeighbor = neighbor.transform.position - transform.position;
            float sqrDist = toNeighbor.sqrMagnitude;

            // Check if within field of view
            float cosAngle = Vector3.Dot(transform.forward, toNeighbor.normalized);
            if (cosAngle >= minCos && sqrDist <= manager.NeighbourDistanceSqr)
            {
                center += neighbor.transform.position;
                groupSize++;
            }
        }

        if (groupSize > 0)
        {
            center /= groupSize;
            return (center - transform.position);
        }

        return Vector3.zero;
    }

    private void RotateTowardsCenter(FlockingManager manager)
    {
        Vector3 direction = manager.transform.position - transform.position;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            manager.RotationSpeed * Time.deltaTime
        );
    }

    private void RecalculateSpeed(FlockingManager manager)
    {
        if (Random.Range(0, 100) < 10f)
            _movementSpeed = Random.Range(manager.MinSpeed, manager.MaxSpeed);
    }

    public Vector3 LastPosition => _lastPosition;
    public void UpdateLastPosition() => _lastPosition = transform.position;
}