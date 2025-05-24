using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    //private float _movementSpeed;

    //public void Initialize(FlockingManager manager)
    //{
    //    _movementSpeed = Random.Range(manager.MinSpeed, manager.MaxSpeed);
    //}

    //public void UpdateBehaviour(FlockingManager manager, Octree octree)
    //{
    //    HandleFishBoundery(manager);
    //    ApplyFlockingRules(manager, octree);
    //    MoveFish();
    //}

    //private void MoveFish()
    //{
    //    this.transform.Translate(0, 0, _movementSpeed * Time.deltaTime);
    //}

    //private void HandleFishBoundery(FlockingManager manager)
    //{
    //    if (!IsWithinBounds(manager))
    //    {
    //        RotateTowardsCenter(manager);
    //    }
    //}

    //private void ApplyFlockingRules(FlockingManager manager, Octree octree)
    //{
    //    if (manager == null || octree == null) return;

    //    // Query the octree for neighbors within the neighbor distance
    //    List<Transform> nearbyObjects = octree.QueryNeighbors(
    //        this.transform.position,
    //        manager.NeighbourDistance
    //    );

    //    // Convert GameObjects to Flock components (and skip self)
    //    List<Flock> neighbors = new List<Flock>();
    //    foreach (var obj in nearbyObjects)
    //    {
    //        Flock flock = obj.GetComponent<Flock>();
    //        if (flock != null && flock != this)
    //        {
    //            neighbors.Add(flock);
    //        }
    //    }

    //    Vector3 separation = CalculateSeparation(neighbors.ToArray());
    //    Vector3 alignment = CalculateAlignment(neighbors.ToArray(), manager);
    //    Vector3 cohesion = CalculateCohesion(neighbors.ToArray(), manager);

    //    Vector3 direction =
    //    separation * manager.SeparationWeight +
    //    alignment * manager.AlignmentWeight +
    //    cohesion * manager.CohesionWeight;

    //    if (direction != Vector3.zero)
    //    {
    //        this.transform.rotation = Quaternion.Slerp(
    //            this.transform.rotation,
    //            Quaternion.LookRotation(direction),
    //            manager.RotationSpeed * Time.deltaTime
    //        );
    //    }
    //}

    //private Vector3 CalculateSeparation(Flock[] neighbors)
    //{
    //    Vector3 separationVector = Vector3.zero;
    //    int avoidCount = 0;

    //    foreach (Flock fish in neighbors)
    //    {
    //        if (fish == this) continue;

    //        float distance = (fish.transform.position - this.transform.position).sqrMagnitude;
    //        if (distance < 1.0f)
    //        {
    //            separationVector += (this.transform.position - fish.transform.position);
    //            avoidCount++;
    //        }
    //    }

    //    if (avoidCount > 0)
    //    {
    //        separationVector /= avoidCount;
    //    }

    //    return separationVector;
    //}

    //private Vector3 CalculateAlignment(Flock[] neighbors, FlockingManager manager)
    //{
    //    Vector3 alignmentVector = Vector3.zero;
    //    int alignmentCount = 0;

    //    foreach (Flock fish in neighbors)
    //    {
    //        if (fish == this) continue;

    //        Vector3 toNeighbor = fish.transform.position - this.transform.position;
    //        float distance = toNeighbor.sqrMagnitude;

    //        if (distance <= manager.NeighbourDistance * manager.NeighbourDistance)
    //        {
    //            float angle = Vector3.Angle(this.transform.forward, toNeighbor);
    //            if (angle <= manager.ViewingAngle * 0.5f)
    //            {
    //                alignmentVector += fish.transform.forward;
    //                alignmentCount++;
    //            }
    //        }
    //    }

    //    if (alignmentCount > 0)
    //    {
    //        alignmentVector /= alignmentCount;
    //    }

    //    return alignmentVector;
    //}

    //private Vector3 CalculateCohesion(Flock[] neighbors, FlockingManager manager)
    //{
    //    Vector3 center = Vector3.zero;
    //    int groupSize = 0;
    //    float totalSpeed = 0f;

    //    foreach (Flock fish in neighbors)
    //    {
    //        if (fish == this) continue;

    //        Vector3 toNeighbor = fish.transform.position - this.transform.position;
    //        float distance = toNeighbor.sqrMagnitude;

    //        if (distance <= manager.NeighbourDistance * manager.NeighbourDistance)
    //        {
    //            float angle = Vector3.Angle(this.transform.forward, toNeighbor);
    //            if (angle <= manager.ViewingAngle * 0.5f)
    //            {
    //                center += fish.transform.position;
    //                groupSize++;
    //                totalSpeed += fish._movementSpeed;
    //            }
    //        }
    //    }

    //    if (groupSize > 0)
    //    {
    //        center /= groupSize;
    //        float averageSpeed = totalSpeed / groupSize;
    //        _movementSpeed = Mathf.Clamp(averageSpeed, manager.MinSpeed, manager.MaxSpeed);

    //        return (center - this.transform.position); 
    //    }

    //    return Vector3.zero;
    //}

    //private void RotateTowardsCenter(FlockingManager manager)
    //{
    //    Vector3 direction = manager.transform.position - this.transform.position;
    //    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction),
    //                                                                        manager.RotationSpeed * Time.deltaTime);
    //}

    //private bool IsWithinBounds(FlockingManager manager)
    //{
    //    return manager.FishTankBounds.Contains(this.transform.position);
    //}

    //private void RecalculateSpeed(FlockingManager manager)
    //{
    //    _movementSpeed = Random.Range(manager.MinSpeed, manager.MaxSpeed);
    //}

    


}
