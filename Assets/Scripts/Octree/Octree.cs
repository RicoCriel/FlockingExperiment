using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    internal OctreeNode _root;
    private readonly Dictionary<GameObject, OctreeNode> _objectToNodeMap;
    private readonly int _maxObjectsPerNode = 10; 
    private readonly float _minNodeSize = 0.5f;

    public Octree(Bounds bounds)
    {
        _root = new OctreeNode(bounds, _maxObjectsPerNode, _minNodeSize);
        _objectToNodeMap = new Dictionary<GameObject, OctreeNode>();
    }

    public void Insert(GameObject obj)
    {
        OctreeNode node = _root.InsertAndReturnNode(obj);
        if (node != null) _objectToNodeMap[obj] = node;
    }

    public void BatchInsert(IEnumerable<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            Insert(obj);
        }
    }

    public void Remove(GameObject obj)
    {
        if (_objectToNodeMap.TryGetValue(obj, out OctreeNode node))
        {
            node.Remove(obj);
            _objectToNodeMap.Remove(obj);
            node.TryCollapse();
        }
    }

    public void UpdatePosition(GameObject obj, Vector3 lastPosition)
    {
        // Only update if moved a lot
        if ((obj.transform.position - lastPosition).sqrMagnitude < 0.01f) return;

        if (_objectToNodeMap.TryGetValue(obj, out OctreeNode currentNode))
        {
            if (!currentNode.Bounds.Contains(obj.transform.position))
            {
                Remove(obj);
                Insert(obj);
            }
        }
    }

    public void QueryNeighbors(Vector3 point, float radius, List<GameObject> results)
    {
        _root.QueryNeighbors(point, radius, results);
    }
}
