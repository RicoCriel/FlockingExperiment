using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct OctreeData
{
    public float3 Position;
    public int Index;
}

public class Octree
{
    private OctreeNode _root;
    private readonly int _maxObjectsPerNode;
    private readonly float _minNodeSize;

    public Octree(Bounds bounds, int maxObjectsPerNode = 10, float minNodeSize = 0.5f)
    {
        _root = new OctreeNode(bounds, maxObjectsPerNode, minNodeSize);
        _maxObjectsPerNode = maxObjectsPerNode;
        _minNodeSize = minNodeSize;
    }

    public void Rebuild(NativeArray<Matrix4x4> fishData)
    {
        _root = new OctreeNode(_root.Bounds, _maxObjectsPerNode, _minNodeSize);

        for (int i = 0; i < fishData.Length; i++)
        {
            var fish = new OctreeData
            {
                Position = fishData[i].GetPosition(),
                Index = i
            };
            _root.Insert(fish);
        }
    }

    public void QueryNeighbors(float3 position, float radius, NativeList<int> results)
    {
        _root.QueryNeighbors(position, radius, results);
    }
}

