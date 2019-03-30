using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Grid : MonoBehaviour
{
    public PlayerNumber playerNumber;
    public Transform spawnPoint;
    public Transform target;

    private AstarPath _path;
    private GridGraph _graph;
    private Vector3 _start;
    private Vector3 _end;
    private GraphNode _startNode;
    private GraphNode _endNode;
    private BoxCollider _clickCollider;
    private GameObject _clickObj;
    private RaycastHit[] _hitResults = new RaycastHit[5];

    private const int kInterval = 2;
    private const int kHalfInterval = kInterval / 2;

    private int _clickLayer;
    
    // Use this for initialization
    void Start ()
    {
        _path = GetComponent<AstarPath>();

        int graphIndex = (byte)playerNumber - 1;
        _graph = (_path.graphs[graphIndex]) as GridGraph;

        _startNode = _graph.GetNearest(spawnPoint.position).node;
        _endNode = _graph.GetNearest(target.position).node;

        _clickLayer = LayerMask.NameToLayer("click_check");

        _clickObj = new GameObject("clickObj");
        _clickObj.layer = _clickLayer;
        _clickCollider = _clickObj.AddComponent<BoxCollider>();
        _clickCollider.isTrigger = true;
        _clickCollider.center = _graph.center;
        _clickCollider.size = new Vector3(_graph.Width, 0.1f, _graph.Depth) * _graph.nodeSize;
        _clickObj.transform.SetParent(transform);


	}
    
    public GridPosition GetGridPosition(Vector3 closestPosition)
    {
        int mx = kInterval * Mathf.RoundToInt(closestPosition.x / kInterval);
        int mz = kInterval * Mathf.RoundToInt(closestPosition.z / kInterval);

        GridPosition tmp = GridPosition.Create(mx, mz);
        Debug.DrawRay(tmp.ToVector3(), Vector3.up, Color.grey);

        return GridPosition.Create(mx, mz);
    }

    public bool CanBuildTower(Ray rayToGrid, bool preventBlocking, out GridPosition gridPosition)
    {
        bool result = false;
        gridPosition = GridPosition.Create(0, 0);
        
        if (Physics.RaycastNonAlloc(rayToGrid, _hitResults, 200.0f, ~_clickLayer, QueryTriggerInteraction.Collide) > 0)
        {
            RaycastHit hit = _hitResults[0]; // Only care about the first one

            Debug.DrawRay(hit.point, Vector3.up, Color.blue);

            gridPosition = GetGridPosition(hit.point);
            Color color = result ? Color.green : Color.red;
            Debug.DrawRay(gridPosition.ToVector3(), Vector3.up, color);

            if(preventBlocking)
            {
                if(CanBuildTowerAtPos(gridPosition))
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }
        }

        return result;
    }

    public bool CanBuildTowerAtPos(GridPosition gridPosition)
    {
        NNInfoInternal nodeInfo = _graph.GetNearest(gridPosition.ToVector3());
        GraphNode nodeAtPos = nodeInfo.node;
        bool isWalkable = nodeAtPos.Walkable;

        bool isBuildable = nodeAtPos.Tag != 1 && nodeAtPos.Tag != 2;
        
        Bounds testBounds = _getApproximateBoundsFromGridPos(gridPosition);
        GraphUpdateObject guo = new GraphUpdateObject(testBounds);
        guo.modifyWalkability = true;
        guo.setWalkability = false;

        bool wontBlock = GraphUpdateUtilities.UpdateGraphsNoBlock(guo, _startNode, _endNode, true);
       
        return isWalkable && isBuildable && wontBlock;
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
