using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private OctreeNode _root;
    private Dictionary<GameObject, OctreeNode> _objectToNodeMap;
    private int _maxObjectsPerNode = 30;

    public Octree(Bounds bounds)
    {
        _root = new OctreeNode(bounds, _maxObjectsPerNode);
        _objectToNodeMap = new Dictionary<GameObject, OctreeNode>();
    }

    public void Insert(GameObject obj)
    {
        OctreeNode node = _root.InsertAndReturnNode(obj);
        if (node != null) _objectToNodeMap[obj] = node;
    }

    public void Remove(GameObject obj)
    {
        if (_objectToNodeMap.TryGetValue(obj, out OctreeNode node))
        {
            node.Remove(obj);
            _objectToNodeMap.Remove(obj);
        }
    }

    public void UpdatePosition(GameObject obj)
    {
        if (!_objectToNodeMap.TryGetValue(obj, out OctreeNode currentNode)) return;
        if (!currentNode.Bounds.Contains(obj.transform.position))
        {
            Remove(obj);
            Insert(obj);
        }
    }

    public List<GameObject> QueryNeighbors(Vector3 point, float radius)
    {
        List<GameObject> neighbors = new List<GameObject>();
        _root.QueryNeighbors(point, radius, neighbors);
        return neighbors;
    }
}