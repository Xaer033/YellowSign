using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using DG.Tweening;

[RequireComponent(typeof(TSTransform), typeof(Collider))]
public class BasicTowerView : MonoBehaviour, ITowerView
{
    public GameObject fxPrefab;
    public Transform cannonHook;

    private TSTransform _transform;
    private Collider _collider;

    private TrailRenderer _fxInstance;
    private Sequence _fxTween;

    void Awake()
    {
        _transform = GetComponent<TSTransform>();
        _collider = GetComponent<Collider>();

        GameObject fxObj = GameObject.Instantiate<GameObject>(fxPrefab, transform.position, transform.rotation);
        _fxInstance = fxObj.GetComponent<TrailRenderer>();
        _fxInstance.gameObject.SetActive(false);
    }

    public Tower tower { get; set; }

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

    public FP VisualAttack(ICreepView target)
    {
        const float kDuration = 0.105f;
        //tower.stats.attackType.
        if(_fxTween != null)
        {
            _fxTween.Kill(true);
            _fxTween = null;
        }

        _fxInstance.gameObject.SetActive(true);
        _fxInstance.transform.position = cannonHook.position;

        Vector3 forwardVec = target.transformTS.forward.ToVector() * target.creep.stats.baseSpeed * kDuration;
        Vector3 tPos = target.targetPosition + forwardVec;
        _fxTween = DOTween.Sequence();
        Tween moveTween = _fxInstance.transform.DOMove(tPos, kDuration);

        _fxTween.Insert(0.0f, moveTween);
        _fxTween.InsertCallback(kDuration + 0.25f, onTrailComplete);

        return (FP)kDuration;
    }

    private void onTrailComplete()
    {
        _fxInstance.gameObject.SetActive(false);
        _fxTween = null;
    }
}
