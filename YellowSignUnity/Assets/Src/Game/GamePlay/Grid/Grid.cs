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
    private GraphNode _startNode;
    private GraphNode _endNode;
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

        _startNode = _graph.GetNearest(spawnPoints[0].position).node;
        _endNode = _graph.GetNearest(target.position).node;

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
            if(CanBuildTowerAtPos(gridPosition))
            {
                result = true;
            }

            Color color = result ? Color.green : Color.red;
            Debug.DrawRay(gridPosition.ToVector3(), Vector3.up, color);
        }

        return result;
    }

    public bool CanBuildTowerAtPos(GridPosition gridPosition)
    {

        GraphNode nodeAtPos = _graph.GetNearest(gridPosition.ToVector3()).node;
        bool isWalkable = nodeAtPos.Walkable;
        Debug.Log("Log: " + nodeAtPos.Tag);

        Bounds testBounds = _getApproximateBoundsFromGridPos(gridPosition);
        GraphUpdateObject guo = new GraphUpdateObject(testBounds);
        guo.modifyWalkability = true;
        guo.setWalkability = false;
        return GraphUpdateUtilities.UpdateGraphsNoBlock(guo, _startNode, _endNode, true);
    }

    public void UpdateGridPosition(Bounds bounds)
    {
        _path.UpdateGraphs(bounds);
    }

    private Bounds _getApproximateBoundsFromGridPos(GridPosition pos)
    {
        return new Bounds(pos.ToVector3(), Vector3.one * 0.75f);
    }
}
