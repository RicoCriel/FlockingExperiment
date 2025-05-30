using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class OctreeNode
{
    public Bounds Bounds;
    public List<OctreeData> Objects;
    public OctreeNode[] Children;
    private float _minSize;
    private int _maxObjects;

    public OctreeNode(Bounds bounds, int maxObjects, float minSize)
    {
        Bounds = bounds;
        Objects = new List<OctreeData>();
        _maxObjects = maxObjects;
        _minSize = minSize;
    }

    public void Insert(OctreeData fish)
    {
        if (!Bounds.Contains(fish.Position)) return;

        if (Children == null)
        {
            Objects.Add(fish);
            if (Objects.Count > _maxObjects && Bounds.size.x > _minSize * 2)
            {
                Subdivide();
            }
        }
        else
        {
            foreach (var child in Children)
            {
                child.Insert(fish);
            }
        }
    }

    private void Subdivide()
    {
        Vector3 size = Bounds.size / 2f;
        if (size.x < _minSize) return;

        Children = new OctreeNode[8];
        Vector3 center = Bounds.center;

        for (int i = 0; i < 8; i++)
        {
            Vector3 offset = new Vector3(
                (i & 1) == 0 ? -size.x / 2 : size.x / 2,
                (i & 2) == 0 ? -size.y / 2 : size.y / 2,
                (i & 4) == 0 ? -size.z / 2 : size.z / 2
            );

            Children[i] = new OctreeNode(new Bounds(center + offset, size), _maxObjects, _minSize);
        }

        // Redistribute objects
        foreach (var fish in Objects)
        {
            bool placed = false;
            foreach (var child in Children)
            {
                if (child.Bounds.Contains(fish.Position))
                {
                    child.Insert(fish);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                // If fish doesn't fit in any child, keep in parent
                Objects.Add(fish);
            }
        }

        Objects.Clear();
    }

    public void QueryNeighbors(float3 position, float radius, NativeList<int> results)
    {
        float radiusSqr = radius * radius;
        float3 closestPoint = Bounds.ClosestPoint(position);
        float sqrDistToBounds = math.lengthsq(closestPoint - position);

        if (sqrDistToBounds > radiusSqr) return;

        if (Children != null)
        {
            foreach (var child in Children)
            {
                child.QueryNeighbors(position, radius, results);
            }
        }
        else
        {
            foreach (var fish in Objects)
            {
                float sqrDist = math.lengthsq(fish.Position - position);
                if (sqrDist <= radiusSqr)
                {
                    results.Add(fish.Index);
                }
            }
        }
    }
}


