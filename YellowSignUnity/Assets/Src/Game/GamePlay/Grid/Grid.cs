using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Grid : MonoBehaviour
{
    public Transform[] spawnPoints;
    public Transform target;

    private AstarPath _path;
    private GridGraph _graph;
    private Vector3 _start = new Vector3(0, 4, 5);
    private Vector3 _end = new Vector3(0, 0.0f, 8);
    private BoxCollider _clickCollider;
    private GameObject _clickObj;

    private const int kInterval = 2;
    private const int kHalfInterval = kInterval / 2;

    private int _clickLayer;
    // Use this for initialization
    void Start ()
    {
        _path = GetComponent<AstarPath>();
        _graph = (_path.graphs[0]) as GridGraph;
        _clickLayer = LayerMask.NameToLayer("click_check");

        _clickObj = new GameObject("clickObj");
        _clickObj.layer = _clickLayer;
        _clickCollider = _clickObj.AddComponent<BoxCollider>();
        _clickCollider.isTrigger = true;
        _clickCollider.center = _graph.center;
        _clickCollider.size = new Vector3(_graph.Width, 0.1f, _graph.Depth) * _graph.nodeSize;
        _clickObj.transform.SetParent(transform);
        
        //_path.UpdateGraphs( new Bounds(Vector3.forward * 7.0f, Vector3.one));

        //_graph
	}
    
    public GridPosition GetGridPosition(Vector3 closestPosition)
    {
        int mx = (int)Mathf.Round(closestPosition.x);
        int mz = (int)Mathf.Round(closestPosition.z);
        
        return GridPosition.Create( mx - ((mx / kHalfInterval) % kInterval), 
                                    mz - ((mz / kHalfInterval) % kInterval));
    }

    public bool CanBuildTower(Ray rayToGrid, out GridPosition gridPosition)
    {
        bool result = false;
        gridPosition = GridPosition.Create(0, 0);

        RaycastHit hit;
        if (Physics.Raycast(rayToGrid, out hit, 100.0f, ~_clickLayer, QueryTriggerInteraction.Collide))
        {
            gridPosition = GetGridPosition(hit.point);
            result = (_graph.GetNearest(gridPosition.ToVector3()).node.Walkable);

            Color color = result ? Color.green : Color.red;
            Debug.DrawRay(gridPosition.ToVector3(), Vector3.up, color);
        }

        return result;
    }

    public void UpdateGridPosition(Bounds bounds)
    {
        _path.UpdateGraphs(bounds);
    }
}
