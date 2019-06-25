using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ActorSelector : GhostGen.EventDispatcher
{
    
    
    public const float MAG_DIST = 5f;
    
    private byte _ownerId;
    private Camera _camera;
    private int _maxSelection;
    private RaycastHit[] _hitResults;
    private Vector3 _dragStart;
    private Vector3 _dragEnd;
    private bool _dragStartEventSent;

    public struct DragEndEventData
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
    }
    
    public ActorSelector(byte ownerId, Camera camera, int maxSelection)
    {
        _ownerId = ownerId;
        _camera = camera;
        _maxSelection = maxSelection;
        _hitResults = new RaycastHit[_maxSelection];
        _dragStartEventSent = false;
    }

    public void Tick()
    {
        if (_camera == null)
        {
            Debug.LogError("Camera not set!");
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            _dragStart = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 clickPos = Input.mousePosition;
            if (_dragStartEventSent)
            {
                _dragEnd = clickPos;
                
                DragEndEventData dragData = new DragEndEventData();
                dragData.startPoint = _dragStart;
                dragData.endPoint = _dragEnd;
                DispatchEvent(PlayerUIEventType.DRAG_END, false, dragData);
            }
            else
            {
                DispatchEvent(PlayerUIEventType.PRIMARY_SELECT, false, clickPos);
            }

            _dragStart = Vector3.zero;
            _dragEnd = Vector3.zero;
            _dragStartEventSent = false; 
        }

        if (Input.GetMouseButtonUp(1))
        {
            Vector3 clickPos = Input.mousePosition;
            DispatchEvent(PlayerUIEventType.SECONDARY_SELECT, false, clickPos);
        }

        if (Input.GetMouseButton(0))
        {
            if (!_dragStartEventSent)
            {
                float mag = (Input.mousePosition - _dragStart).sqrMagnitude;
                Debug.Log("Mag: " + mag);
                if (mag > MAG_DIST)
                {
                    DispatchEvent(PlayerUIEventType.DRAG_BEGIN, false, _dragStart);
                    _dragStartEventSent = true;
                }
            }
        }
    }
    public bool PickSelector<T>(Ray ray, int validLayers, out T view) where T: IActor
    {
        bool result = false;
        view = default(T);

        int hitCount = Physics.RaycastNonAlloc(ray, _hitResults, 2000.0f, validLayers, QueryTriggerInteraction.Collide);
        if (hitCount > 0)
        {
            RaycastHit hit = _hitResults[0]; // Only care about the first one
            view = hit.transform.GetComponent<T>();
            result = true;
        }

        return result;
    }
    
    public bool MultiPickSelector<T>(BoxCastParam param, int validLayers, ref List<T> viewList) where T: IActor
    {
        bool result = false;

        if (viewList == null)
        {
            Assert.IsTrue(false, "View List for actor picker is null!");
        }
        else
        {
            viewList.Clear();
            
            int hitCount = Physics.BoxCastNonAlloc(param.center, param.halfExtends, param.direction, _hitResults, Quaternion.identity, 2000.0f, validLayers, QueryTriggerInteraction.Collide);
            for(int i = 0; i < hitCount; ++i)
            {
                if (i >= _hitResults.Length)
                {
                    break;
                }
                
                RaycastHit hit = _hitResults[i]; // Only care about the first one
                T view = hit.transform.GetComponent<T>();
                if (!EqualityComparer<T>.Default.Equals(view, default(T)))
                {
                    viewList.Add(view);
                }
                result = true;
            }
        }

        return result;
    }
}
