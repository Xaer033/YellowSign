using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Pathfinding;
using Zenject;
using GhostGen;


public class Creep : EventDispatcher, IAttacker, IAttackTarget
{
    public class Factory : PlaceholderFactory<string, CreepSpawnInfo, Creep>, IValidatable
    {
        private DiContainer _container;
        private CreepDictionary _creepDefs;
        private CreepSystem _creepSystem;

        public Factory(
            CreepDictionary creepDefs, 
            CreepSystem creepSystem, 
            DiContainer container)
        {
            _container = container;
            _creepDefs = creepDefs;
            _creepSystem = creepSystem;
        }

        public override Creep Create(string creepId, CreepSpawnInfo spawnInfo)
        {
            CreepDef def = _creepDefs.GetDef(creepId);
            Creep creep = _container.Instantiate<Creep>(new object[] { def, spawnInfo });
            _creepSystem.AddCreep(spawnInfo.ownerId, creep);
            
            return creep;
        }

        public override void Validate()
        {
            //TowerDef def = _towerDefs.GetDef("basic_tower");
            _container.Instantiate<Creep>(new object[] { null, null });
        }
    }

    public FP REPATH_RATE = 0.75f;

    public CreepState state { get; set; }
    public CreepStats stats { get; set; }

    public TSTransform transform { get { return _transform; } }
    public ICreepView view { get; private set; }

    public bool flagForRemoval { get;  set; }
    public bool reachedTarget { get; set; }

    public byte ownerId         { get; private set; }
    public byte targetOwnerId   { get; private set; }

    private Seeker _seeker;
    private bool _canSearchAgain;
    private TSTransform _transform;
    private FP _nextRepath;

    private List<TSVector> _vectorPath;
    private int _waypointIndex;
    private FP _distanceToNextWaypoint = 0.35f;

    private FP _drag = 5f;
    private TSVector _targetPosition;
    private Path _path = null;
    private FP _speed;

    public Creep(CreepDef def, CreepSpawnInfo spawnInfo)
    {
        _nextRepath = 0;
        _waypointIndex = 0;
        _canSearchAgain = true;
        reachedTarget = false;
        _vectorPath = new List<TSVector>();

        ownerId = spawnInfo.ownerId;

        // Move outside creep state
        GameObject creepObj = TrueSyncManager.SyncedInstantiate(
            def.view.gameObject, 
            spawnInfo.position, 
            spawnInfo.rotation);
        
        view = creepObj.GetComponent<ICreepView>();
        view.creep = this;
        
        stats = def.stats;
        state = CreepState.CreateFromStats(stats);

        _transform = view.transformTS;
        _seeker = view.seeker;

        _speed = stats.baseSpeed;
        _targetPosition = spawnInfo.targetPosition;
        targetOwnerId = spawnInfo.targetOwnerId;

        
        RecalculatePath();
    }

    public bool isValid
    {
        get
        {
            return (view != null && !isDead);
        }
    }

    public int health
    {
        get { return state.health; }
    }

    public bool isDead
    {
        get { return state.health <= 0; }
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
        
        AttackResult result = new AttackResult(
            this,
            totalDamage, 
            newTargetHealth, 
            state.isDead);

        DispatchEvent(GameplayEventType.CREEP_DAMAGED, true, result);

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

        Path result = _seeker.StartPath(
                        _transform.position.ToVector(), 
                        _targetPosition.ToVector(), 
                        onPathComplete);

        return result;
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

        //FP d = (p2 - p1).magnitude;
        _waypointIndex = 0;

        _vectorPath.Clear();
        for(int i = 0; i < p.vectorPath.Count; ++i)
        {
            _vectorPath.Add(p.vectorPath[i].ToTSVector());
        }
    }
}
