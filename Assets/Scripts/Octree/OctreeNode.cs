using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    public Bounds Bounds;
    public List<GameObject> Objects;
    public OctreeNode[] Children;
    private float _minSize;
    private int _maxObjects;

    public OctreeNode(Bounds bounds, int maxObjects, float minSize)
    {
        Bounds = bounds;
        Objects = new List<GameObject>();
        _maxObjects = maxObjects;
        _minSize = minSize;
    }

    public OctreeNode InsertAndReturnNode(GameObject obj)
    {
        if (obj == null) return null;
        if (!Bounds.Contains(obj.transform.position)) return null;

        if (Children == null)
        {
            Objects.Add(obj);
            if (Objects.Count > _maxObjects && Bounds.size.x > _minSize * 2)
            {
                Subdivide();
            }
            return this;
        }
        else
        {
            foreach (var child in Children)
            {
                OctreeNode insertedNode = child.InsertAndReturnNode(obj);
                if (insertedNode != null) return insertedNode;
            }
            // Fallback: object stays in parent node
            Objects.Add(obj);
            return this;
        }
    }

    public void Subdivide()
    {
        Vector3 size = Bounds.size / 2f;
        if (size.x < _minSize || size.y < _minSize || size.z < _minSize)
            return;

        Children = new OctreeNode[8];
        Vector3 center = Bounds.center;

        for (int i = 0; i < 8; i++)
        {
            Vector3 offset = new Vector3(
                (i & 1) == 0 ? -size.x / 2 : size.x / 2,
                (i & 2) == 0 ? -size.y / 2 : size.y / 2,
                (i & 4) == 0 ? -size.z / 2 : size.z / 2
            );

            Bounds childBounds = new Bounds(center + offset, size);
            Children[i] = new OctreeNode(childBounds, _maxObjects, _minSize);
        }

        // Redistribute objects
        List<GameObject> objectsToRedistribute = new List<GameObject>(Objects);
        Objects.Clear();

        foreach (var obj in objectsToRedistribute)
        {
            bool placedInChild = false;
            foreach (var child in Children)
            {
                if (child.Bounds.Contains(obj.transform.position))
                {
                    child.Objects.Add(obj);
                    placedInChild = true;
                    break;
                }
            }

            // If object doesn't fit in any child, keep in parent
            if (!placedInChild) Objects.Add(obj);
        }
    }

    public void QueryNeighbors(Vector3 point, float radius, List<GameObject> results)
    {
        float radiusSqr = radius * radius;
        Vector3 closestPoint = Bounds.ClosestPoint(point);
        float sqrDistToBounds = (closestPoint - point).sqrMagnitude;

        if (sqrDistToBounds > radiusSqr) return;

        foreach (var obj in Objects)
        {
            float sqrDist = (obj.transform.position - point).sqrMagnitude;
            if (sqrDist <= radiusSqr) results.Add(obj);
        }

        if (Children != null)
        {
            foreach (var child in Children)
            {
                child.QueryNeighbors(point, radius, results);
            }
        }
    }

    public bool Remove(GameObject obj)
    {
        if (Objects.Remove(obj)) return true;

        if (Children != null)
        {
            foreach (var child in Children)
            {
                if (child.Remove(obj)) return true;
            }
        }
        return false;
    }

    public void TryCollapse()
    {
        if (Children == null) return;

        int totalObjects = Objects.Count;
        foreach (var child in Children)
        {
            if (child.Children != null) return; // Can't collapse if grandchildren exist
            totalObjects += child.Objects.Count;
        }

        if (totalObjects <= _maxObjects)
        {
            // Move all child objects to parent
            foreach (var child in Children)
            {
                Objects.AddRange(child.Objects);
            }
            Children = null;
        }
    }
}


