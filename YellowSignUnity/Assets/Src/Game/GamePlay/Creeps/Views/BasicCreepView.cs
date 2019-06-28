using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Pathfinding;

public class BasicCreepView : MonoBehaviour, ICreepView
{
    public Transform targetTransform;
    public Transform healthTransform;

    private TSTransform _transform;
    private Collider _collider;
    private Seeker _seeker;
    private bool _isSelected;

    void Awake()
    {
        _isSelected = false;
        _transform = GetComponent<TSTransform>();
        _collider = GetComponent<Collider>();
        _seeker = GetComponent<Seeker>();
    }

    public Creep creep { get; set; }

    public TSTransform transformTS
    {
        get { return _transform; }
    }

    public Bounds bounds
    {
        get { return _collider.bounds; }
    }

    public TSVector position
    {
        get { return _transform.position; }
    }

    public TSQuaternion rotation
    {
        get { return _transform.rotation; }
    }

    public bool isSelected
    {
        get { return _isSelected; }
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
            }
        }
    }
    public Seeker seeker
    {
        get { return _seeker; }
    }

    public Vector3 targetPosition
    {
        get
        {
            return targetTransform != null ? targetTransform.position : Vector3.zero;
        }
    }

    public Vector3 healthPosition
    {
        get { return healthTransform.position; }
    }
}
