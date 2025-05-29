using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    public Bounds Bounds;
    public List<GameObject> Objects;
    public OctreeNode[] Children;
    private int _maxObjects;

    public OctreeNode(Bounds bounds, int maxObjects)
    {
        Bounds = bounds;
        Objects = new List<GameObject>();
        _maxObjects = maxObjects;
    }

    public void BatchInsert(List<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            InsertAndReturnNode(obj);
        }
        SubdivideIfNeeded();
    }

    private void SubdivideIfNeeded()
    {
        if (Children == null && Objects.Count > _maxObjects)
        {
            Subdivide();
        }
    }

    public OctreeNode InsertAndReturnNode(GameObject obj)
    {
        if (!Bounds.Contains(obj.transform.position)) return null;

        if (Objects.Count < _maxObjects || Children == null)
        {
            Objects.Add(obj);
            return this;
        }
        else
        {
            if (Children == null) Subdivide();
            foreach (var child in Children)
            {
                OctreeNode insertedNode = child.InsertAndReturnNode(obj);
                if (insertedNode != null) return insertedNode;
            }
            return null; // Should never happen
        }
    }

    public void Subdivide()
    {
        Children = new OctreeNode[8];
        Vector3 size = Bounds.size / 2f;
        Vector3 center = Bounds.center;

        for (int i = 0; i < 8; i++)
        {
            Vector3 offset = new Vector3(
                (i & 1) == 0 ? -size.x / 2 : size.x / 2,
                (i & 2) == 0 ? -size.y / 2 : size.y / 2,
                (i & 4) == 0 ? -size.z / 2 : size.z / 2
            );

            Bounds childBounds = new Bounds(center + offset, size);
            Children[i] = new OctreeNode(childBounds, _maxObjects);
        }

        // Redistribute objects to children
        List<GameObject> objectsToMove = new List<GameObject>(Objects);
        Objects.Clear();
        foreach (var obj in objectsToMove)
        {
            InsertAndReturnNode(obj);
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
        bool removed = Objects.Remove(obj);
        if (removed) return true;

        if (Children != null)
        {
            foreach (var child in Children)
            {
                if (child.Remove(obj)) return true;
            }
        }
        return false;
    }
}
