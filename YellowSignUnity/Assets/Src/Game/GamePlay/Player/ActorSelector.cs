using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GhostGen;
using UnityEngine;
using UnityEngine.Assertions;

public class ActorSelector : EventDispatcher
{

    public const float DOUBLE_CLICK_DELAY = 0.3f;
    public const float MAG_DIST = 5f;
    
    private byte _ownerId;
    private int _maxSelection;
    private RaycastHit[] _hitResults;
    private Collider[] _colliderResults;
    private Vector3 _dragStart;
    private Vector3 _dragEnd;
    private bool _dragStartEventSent;
    private float _lastClickTimestamp;
    private List<IActor> _selectedActors;

    public event Action<Vector3> onPrimarySelect;
    public event Action<Vector3> onPrimaryDoubleSelect;    
    public event Action<Vector3> onSecondarySelect;   
    public event Action<Vector3> onDragBegin;
    public event Action<DragEndEventData> onDragEnd;

    public event Action<ReadOnlyCollection<IActor>> onSelectionChanged;
    
    public ActorSelector(byte ownerId, int maxSelection)
    {
        _selectedActors = new List<IActor>(maxSelection);
        
        _ownerId = ownerId;
        _maxSelection = maxSelection;
        _hitResults = new RaycastHit[_maxSelection];
        _colliderResults = new Collider[_maxSelection];
        _dragStartEventSent = false;
        _lastClickTimestamp = 0;
    }

    public void SelectActor(IActor a)
    {
        if (a == null)
        {
            Debug.LogError("Cannot select actor, actor is null!");
            return;
        }

        a.isSelected = true;
        if (!_selectedActors.Contains(a))
        {
            _selectedActors.Add(a);
            onSelectionChanged(_selectedActors.AsReadOnly());
        }
    }

    public void DeselectActor(IActor a)
    {
        if (a == null)
        {
            Debug.LogError("Cannot de-select actor, actor is null!");
            return;
        }

        a.isSelected = false;
        if (_selectedActors.Contains(a))
        {
            _selectedActors.Remove(a);
            onSelectionChanged(_selectedActors.AsReadOnly());
        }
    }

    public void ClearSelectedActors()
    {
        for (int i = _selectedActors.Count - 1; i >= 0; --i)
        {
            _selectedActors[i].isSelected = false;
        }
        
        _selectedActors.Clear();
        onSelectionChanged(_selectedActors.AsReadOnly());
    }
    
    public void Tick()
    {
        Vector3 mousePosition = Input.mousePosition;
        
        if (Input.GetMouseButtonDown(0))
        {
            _dragStart = mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            bool doubleClicked = Time.unscaledTime - _lastClickTimestamp < DOUBLE_CLICK_DELAY; 
            _lastClickTimestamp = Time.unscaledTime;
            
            if (_dragStartEventSent)
            {
                _dragEnd = mousePosition;
                
                DragEndEventData dragData = new DragEndEventData();
                dragData.startPoint = _dragStart;
                dragData.endPoint = _dragEnd;

                if (onDragEnd != null)
                {
                    onDragEnd(dragData);
                }
            }
            else
            {
                if (doubleClicked)
                {
                    if (onPrimaryDoubleSelect != null)
                    {
                        onPrimaryDoubleSelect(_dragStart);
                    }
                }
                else
                {
                    if (onPrimarySelect != null)
                    {
                        onPrimarySelect(_dragStart);
                    }
                }
            }

            _dragStart = Vector3.zero;
            _dragEnd = Vector3.zero;
            _dragStartEventSent = false; 
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (onSecondarySelect != null)
            {
                onSecondarySelect(mousePosition);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!_dragStartEventSent)
            {
                float mag = (Input.mousePosition - _dragStart).sqrMagnitude;
                if (mag > MAG_DIST)
                {
                    if (onDragBegin != null)
                    {
                        onDragBegin(_dragStart);
                    }
                    _dragStartEventSent = true;
                }
            }
        }
    }
    public bool PickSelector<T>(Ray ray, int validLayers, out T view) where T: IActor
    {
        bool result = false;
        view = default;

        int hitCount = Physics.RaycastNonAlloc(ray, _hitResults, float.PositiveInfinity, validLayers, QueryTriggerInteraction.Collide);
        if (hitCount > 0)
        {
            RaycastHit hit = _hitResults[0]; // Only care about the first one
            view = hit.transform.GetComponent<T>();
            result = true;
        }

        return result;
    }
    
    public bool MultiPickSelector<T>(Vector3 position, float radius, int validLayers, ref List<T> viewList) where T: IActor
    {
        bool result = false;

        if (viewList == null)
        {
            Assert.IsTrue(false, "View List for actor picker is null!");
        }
        else
        {
            viewList.Clear();
            
            
            int hitCount = Physics.OverlapSphereNonAlloc(position, radius, _colliderResults, validLayers, QueryTriggerInteraction.Collide);
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
