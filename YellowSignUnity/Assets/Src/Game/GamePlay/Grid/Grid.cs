using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Grid : MonoBehaviour
{
    private AstarPath _path;
    private GridGraph _graph;
    private Vector3 _start = new Vector3(0, 4, 5);
    private Vector3 _end = new Vector3(0, 0.0f, 8);
    private BoxCollider _clickCollider;
    private GameObject _clickObj;

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

      // if( )
       {
           //Debug.Log(info.node.position + ", " + info.point);
       }
        //_graph
	}
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1.0f;
            
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100.0f, ~_clickLayer, QueryTriggerInteraction.Collide ))
            {
                Debug.Log("Gotcha");
                int interval = 2;
                int halfInterval = interval / 2;
                
                int mx = (int)Mathf.Round(hit.point.x);
                int mz = (int)Mathf.Round(hit.point.z);
                
                Vector3 adjustedPos = new Vector3(
                    mx -((mx / halfInterval) % interval), 
                    0, 
                    mz - ((mz / halfInterval) % interval));
            
                Color color = (_graph.GetNearest(adjustedPos).node.Walkable) ? Color.green : Color.red;
                Debug.DrawRay(adjustedPos, Vector3.up * 2, color, 3.0f);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(_start, _end);

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Gizmos.DrawRay(ray);
    }
       private void _constructLogicGrid()
    {

    }
}
