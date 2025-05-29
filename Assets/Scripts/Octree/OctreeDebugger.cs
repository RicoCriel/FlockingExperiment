using UnityEngine;

public class OctreeDebugger: MonoBehaviour
{
    public FlockingManager Manager;

    private void Awake()
    {
        Manager = GetComponent<FlockingManager>();
    }

    //private void OnDrawGizmos()
    //{
    //    if (Manager != null && Manager.Octree != null)
    //    {
    //        Manager.Octree.DrawGizmos();
    //    }
    //}
}
