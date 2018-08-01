using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Pathfinding;

public class Creep : IAttacker, IAttackTarget
{
    public FP REPATH_RATE = 0.75f;

    public CreepState state { get; set; }
    public CreepStats stats { get; set; }

    public TSTransform transform { get { return _transform; } }
    public ICreepView view { get; private set; }

    public bool flagForRemoval { get;  set; }
    public bool reachedTarget { get; set; }

    public byte ownerId { get; set; }
    public byte targetOwnerId { get; set; }

    private Seeker _seeker;
    private bool _canSearchAgain;
    private TSTransform _transform;
    private FP _nextRepath;

    private List<TSVector> _vectorPath;
    private int _waypointIndex;
    private FP _distanceToNextWaypoint = 0.35f;

    private FP _drag = 5f;
    private Vector3 _target;
    private Path _path = null;
    private FP _speed;

    public Creep(byte p_ownerId, CreepStats pStats, ICreepView creepView)
    {
        ownerId = p_ownerId;
        view = creepView;
        stats = pStats;
        state = CreepState.CreateFromStats(pStats);

        _transform = creepView.transformTS;
        _seeker = creepView.seeker;

        _nextRepath = 0;
        _waypointIndex = 0;
        _vectorPath = new List<TSVector>();

        _speed = pStats.baseSpeed;
        creepView.creep = this;
    }

    public void Start(byte p_targetOwnerId, Vector3 target)
    {
        targetOwnerId = p_targetOwnerId;
        _target = target;

        _canSearchAgain = true;
        reachedTarget = false;

        RecalculatePath();
    }

    public int health
    {
        get { return state.health; }
    }

    public bool isDead
    {
        get { return state.health <= 0 || reachedTarget; }
    }

    public AttackData CreateAttackData()
    {
        return default(AttackData);
    }

    public AttackResult TakeDamage(AttackData attackData)
    {
        int totalDamage = attackData.potentialDamage;
        int newTargetHealth = state.health - totalDamage;

        //Debug.Log("TotalDamage: " + totalDamage);
        //Debug.Log("Health Left: " + newTargetHealth);

        state.health = newTargetHealth;
        
        AttackResult result = new AttackResult(this, totalDamage, newTargetHealth, state.isDead);
        return result;
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        if(state.isDead)
        {
            flagForRemoval = true;
            return;
        }

        //if(TrueSyncManager.Time >= _nextRepath && _canSearchAgain)
        //{
        //    RecalculatePath();
        //}

        TSVector pos = _transform.position;
        TSVector force;
        if (/*_canSearchAgain &&*/ _vectorPath != null && _vectorPath.Count != 0)
        {
            FP distanceSquared = _distanceToNextWaypoint * _distanceToNextWaypoint;
            while ((_waypointIndex < _vectorPath.Count - 1 && (pos - _vectorPath[_waypointIndex]).sqrMagnitude < distanceSquared) || _waypointIndex == 0)
            {
                _waypointIndex++;
            }
           
            var p1 = pos;
            var p2 = _vectorPath[_waypointIndex];
            
            TSVector dirNormalized = (p2 - p1).normalized;
            force = dirNormalized * _speed;
            force = force * (1 - fixedDeltaTime * _drag);

            _transform.rotation = TSQuaternion.LookRotation(dirNormalized, _transform.up);
            
            if((pos - _vectorPath[_vectorPath.Count - 1]).sqrMagnitude < 2)
            {
                flagForRemoval = true;
                reachedTarget = true;
            }
        }
        else
        {
            // Stand still
            pos = _transform.position;
            force = TSVector.zero;
        }

        _transform.position += force * fixedDeltaTime;        
    }

    public Path RecalculatePath()
    {
        _canSearchAgain = false;
        return _seeker.StartPath(_transform.position.ToVector(), _target, onPathComplete);
    }

    private void onPathComplete(Path _p)
    {
        ABPath p = _p as ABPath;

        _canSearchAgain = true;

        if(_path != null)
        {
            _path.Release(this);
        }

        _path = p;
        p.Claim(this);

        if (p.error)
        {
            _waypointIndex = 0;
            _vectorPath = null;
            return;
        }

        TSVector p1 = p.originalStartPoint.ToTSVector();
        TSVector p2 = _transform.position;
        p1.y = p2.y;
        FP d = (p2 - p1).magnitude;
        _waypointIndex = 0;

        _vectorPath.Clear();
        for(int i = 0; i < p.vectorPath.Count; ++i)
        {
            _vectorPath.Add(p.vectorPath[i].ToTSVector());
        }
    }
}
