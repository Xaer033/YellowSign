using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using DG.Tweening;

[RequireComponent(typeof(TSTransform), typeof(Collider))]
public class BasicTowerView : MonoBehaviour, ITowerView
{
    private const int kFXPoolSize = 3;

    public GameObject fxPrefab;
    public Transform cannonHook;

    private TSTransform _transformTS;
    private Collider _collider;

    [SerializeField]
    private Material _material;
    [SerializeField]
    private Material _highlighterMaterial;
    
    private TrailRenderer _fxInstance;
    private Sequence _fxTween;
    private Vector3[] _fxArcPositions = new Vector3[7];
    private TrailRenderer[] _fxPool;

    private int _fxIndex;

    void Awake()
    {
        _transformTS = GetComponent<TSTransform>();
        _collider = GetComponent<Collider>();        

        setupFXPool();
    }

    public Tower tower { get; set; }

    public Bounds bounds
    {
        get { return _collider.bounds; }
    }

    public TSVector position
    {
        get { return transformTS.position; }
        set { transformTS.position = value; }
    }

    public TSQuaternion rotation
    {
        get { return transformTS.rotation; }
        set { transformTS.rotation = value; }
    }

    public Material material
    {
        get { return _material; }
    }

    public Material highlighterMaterial
    {
        get { return _highlighterMaterial; }
    }

    public TSTransform transformTS
    {
        get
        {
            if(_transformTS == null)
            {
                _transformTS = GetComponent<TSTransform>();
            }
            return _transformTS;
        }
    }

    public FP VisualAttack(ICreepView target)
    {
        const float kDuration = 0.3f;
        const float kPreFireDelay = 0.01f;
        const float kTimeToAttack = kDuration + kPreFireDelay;

        //tower.stats.attackType.
        //if(_fxTween != null)
        //{
        //    _fxTween.Kill(true);
        //    _fxTween = null;
        //}

        Vector3 startPos = cannonHook.position;

        TrailRenderer fx = getNextFX();
        fx.transform.position = startPos;

        Vector3 forwardVec = target.transformTS.forward.ToVector() * target.creep.stats.baseSpeed * (kTimeToAttack) * 5.0f;
        Vector3 endPos = target.targetPosition + (forwardVec * TrueSyncManager.DeltaTime.AsFloat());

        recalculatePath(startPos, endPos);

        _fxTween = DOTween.Sequence();

        Tweener moveTween = fx.transform.DOPath(
            _fxArcPositions, 
            kDuration, 
            PathType.Linear,
            PathMode.Full3D);// (tPos, kDuration);

        moveTween.OnStart(() =>
        {
            fx.gameObject.SetActive(true);
            fx.emitting = (true);
            if(!target.creep.isValid)
            {
                if(_fxTween != null)
                {
                    _fxTween.Kill(true);
                }
            }
        });

        _fxTween.Insert(kPreFireDelay, moveTween);
        _fxTween.InsertCallback(kTimeToAttack, ()=>
        {
            fx.emitting = (false);
            fx.gameObject.SetActive(false);
            _fxTween = null;
        });

        return (FP)kTimeToAttack;
    }
    
    private void recalculatePath(Vector3 start, Vector3 end)
    {
        const float kUpScale = 1.15f;
        Vector3 upVector = Vector3.up * kUpScale;

        _fxArcPositions[0] = start;
        _fxArcPositions[1] = Vector3.Lerp(start, end, 0.15f) + (upVector * 0.05f);
        _fxArcPositions[2] = Vector3.Lerp(start, end, 0.35f) + (upVector * 0.25f);
        _fxArcPositions[3] = Vector3.Lerp(start, end, 0.5f) + (upVector * 0.4f);
        _fxArcPositions[4] = Vector3.Lerp(start, end, 0.75f) + (upVector * 0.35f);
        _fxArcPositions[5] = Vector3.Lerp(start, end, 0.85f) + (upVector * 0.15f);
        _fxArcPositions[6] = end;
    }
    

    private TrailRenderer getNextFX()
    {
        TrailRenderer fx = _fxPool[_fxIndex];
        _fxIndex = (_fxIndex + 1) % _fxPool.Length;
        return fx;
    }

    private void setupFXPool()
    {
        _fxIndex = 0;
        _fxPool = new TrailRenderer[kFXPoolSize];

        for(int i = 0; i < kFXPoolSize; ++i)
        {
            GameObject fxObj = GameObject.Instantiate<GameObject>(fxPrefab, transform.position, transform.rotation);
            _fxPool[i] = fxObj.GetComponent<TrailRenderer>();
            _fxPool[i].emitting = (false);
        }
    }
}
