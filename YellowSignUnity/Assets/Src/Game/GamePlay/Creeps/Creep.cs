using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Pathfinding;
using Zenject;
using GhostGen;


public class Creep : EventDispatcher, IAttacker, IAttackTarget
{
    public static int sCreepCount = 0;

    public class Factory : PlaceholderFactory<string, CreepSpawnInfo, Creep>, IValidatable
    {
        private DiContainer _container;
        private CreepDictionary _creepDefs;

        public Factory(
            CreepDictionary creepDefs, 
            DiContainer container)
        {
            _container = container;
            _creepDefs = creepDefs;
            //_creepSystem = creepSystem;
        }

        public override Creep Create(string creepId, CreepSpawnInfo spawnInfo)
        {
            CreepDef def = _creepDefs.GetDef(creepId);
            Creep creep = _container.Instantiate<Creep>(new object[] { def, spawnInfo, sCreepCount });
            sCreepCount++;
            //_creepSystem.AddCreep(spawnInfo.ownerId, creep);

            return creep;
        }

        public override void Validate()
        {
            //TowerDef def = _towerDefs.GetDef("basic_tower");
            _container.Instantiate<Creep>(new object[] { null, null, 0 });
        }
    }

    public FP REPATH_RATE = 0.75f;

    public CreepState state { get; set; }
    public CreepStats stats { get; set; }

    //public TSTransform transform { get { return _transform; } }
    public ICreepView view { get; private set; }

    public bool flagForRemoval { get;  set; }
    public bool reachedTarget { get; set; }

    public byte ownerId         { get; private set; }
    public byte targetOwnerId   { get; private set; }
    public int  guid            { get; private set; }

    private Seeker _seeker;
    private bool _canSearchAgain;
    //private TSTransform _transform;
    private FP _nextRepath;

    private List<TSVector> _vectorPath;
    private int _waypointIndex;
    private FP _distanceToNextWaypoint = 0.35;

    private FP _drag = 5;
    private TSVector _targetPosition;
    private Path _path = null;
    private FP _speed;
    private CreepDef _def;

    public Creep(CreepDef def, CreepSpawnInfo spawnInfo, int pGuid)
    {
        guid = pGuid;

        _def = def;

        stats = def.stats;
        state = CreepState.CreateFromStats(stats);

        state.position = spawnInfo.position;
        state.rotation = spawnInfo.rotation;

        _speed = stats.baseSpeed;
        _targetPosition = spawnInfo.targetPosition;
        targetOwnerId = spawnInfo.targetOwnerId;


        ownerId = spawnInfo.ownerId;

        // Move outside creep state
        GameObject creepObj = TrueSyncManager.SyncedInstantiate(
            def.view.gameObject, 
            spawnInfo.position, 
            spawnInfo.rotation);
        
        view = creepObj.GetComponent<ICreepView>();
        view.creep = this;
        

        //_transform = view.transformTS;
        _seeker = view.seeker;
        
        _nextRepath = 0;
        _waypointIndex = 0;
        _canSearchAgain = true;
        reachedTarget = false;
        _vectorPath = new List<TSVector>();


        RecalculatePath();
    }

    public bool isValid
    {
        get
        {
            return (view != null && !isDead && !reachedTarget);
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

    public void Reset(CreepDef def, CreepSpawnInfo spawnInfo)
    {

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
            isDead);

        DispatchEvent(GameplayEventType.CREEP_DAMAGED, true, result);
        //notificationDispatcher.DispatchEvent(GameplayEventType.CREEP_DAMAGED, false, result);

        return result;
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        if(isDead)
        {
            flagForRemoval = true;
            return;
        }

        //if(TrueSyncManager.Time >= _nextRepath && _canSearchAgain)
        //{
        //    RecalculatePath();
        //}

        TSVector pos = state.position;
        TSVector force = TSVector.zero;
        if (/*_canSearchAgain &&*/ _vectorPath != null && _vectorPath.Count != 0)
        {
            FP distanceSquared = _distanceToNextWaypoint * _distanceToNextWaypoint;
            while ((_waypointIndex < _vectorPath.Count - 1 && (pos - _vectorPath[_waypointIndex]).sqrMagnitude < distanceSquared) || _waypointIndex == 0)
            {
                _waypointIndex++;
            }

            if(_waypointIndex < _vectorPath.Count)
            {
                var p1 = pos;
                var p2 = _vectorPath[_waypointIndex];

                TSVector dirNormalized = (p2 - p1).normalized;
                force = dirNormalized * _speed;
                force = force * (1 - fixedDeltaTime * _drag);

                Quaternion rot = Quaternion.LookRotation(dirNormalized.ToVector(), Vector3.up);
                state.rotation = new TSQuaternion(rot.x, rot.y, rot.z, rot.w);

                if((pos - _vectorPath[_vectorPath.Count - 1]).sqrMagnitude < 2)
                {
                    flagForRemoval = true;
                    reachedTarget = true;
                }
            }
            else
            {
                Debug.Log("Seems to be in a bad way...Explode to damage nearby towers");
            }
        }

        state.position = pos + (force * fixedDeltaTime);        
    }

    public Path RecalculatePath()
    {
        _canSearchAgain = false;

        Path result = _seeker.StartPath(
                        state.position.ToVector(), 
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
        TSVector p2 = state.position;

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
