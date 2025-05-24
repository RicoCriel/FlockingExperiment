using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private OctreeNode _root;
    private Dictionary<Transform, OctreeNode> _objectToNodeMap;
    private int _maxObjectsPerNode = 30;

    public Octree(Bounds bounds)
    {
        _root = new OctreeNode(bounds, _maxObjectsPerNode);
        _objectToNodeMap = new Dictionary<Transform, OctreeNode>();
    }

    public void Insert(Transform obj)
    {
        OctreeNode node = _root.InsertAndReturnNode(obj);
        if (node != null) _objectToNodeMap[obj] = node;
    }

    public void Remove(Transform obj)
    {
        if (_objectToNodeMap.TryGetValue(obj, out OctreeNode node))
        {
            node.Remove(obj);
            _objectToNodeMap.Remove(obj);
        }
    }

    public void UpdatePosition(Transform obj)
    {
        if (!_objectToNodeMap.TryGetValue(obj, out OctreeNode currentNode)) return;
        if (!currentNode.Bounds.Contains(obj.transform.position))
        {
            Remove(obj);
            Insert(obj);
        }
    }

    public List<Transform> QueryNeighbors(Vector3 point, float radius)
    {
        List<Transform> neighbors = new List<Transform>();
        _root.QueryNeighbors(point, radius, neighbors);
        return neighbors;
    }

    //public void DrawGizmos()
    //{
    //    _root.DrawGizmos();
    //}
}