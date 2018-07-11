using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

[RequireComponent(typeof(TSTransform), typeof(Collider))]
public class BasicTowerView : MonoBehaviour, ITowerView
{
    private TSTransform _transform;
    private Collider _collider;


    void Awake()
    {
        _transform = GetComponent<TSTransform>();
        _collider = GetComponent<Collider>();
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
    

}
