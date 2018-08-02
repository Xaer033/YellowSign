using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Zenject;


public class Tower : IAttacker, IAttackTarget
{ 
    public class Factory : PlaceholderFactory<string, TSVector, TSQuaternion, Tower>, IValidatable
    {
        private DiContainer _container;
        private TowerDictionary _towerDefs;
        private TowerSystem _towerSystem;

        public Factory(
            TowerDictionary towerDefs, 
            TowerSystem towerSystem, 
            DiContainer container)
        {
            _container = container;
            _towerDefs = towerDefs;
            _towerSystem = towerSystem;
        }

        public override Tower Create(string towerId, TSVector position, TSQuaternion rotation)
        {
            TowerDef def = _towerDefs.GetDef(towerId) as TowerDef;
            GameObject towerGameObject = TrueSyncManager.SyncedInstantiate(def.view.gameObject, position, rotation);
            ITowerView towerView = towerGameObject.GetComponent<ITowerView>();

            Tower tower = _container.Instantiate<Tower>(new object[] { def.stats, towerView, def.brain });
            _towerSystem.AddTower(tower);

            return tower;
        }

        public override void Validate()
        {
            //TowerDef def = _towerDefs.GetDef("basic_tower");
            _container.Instantiate<Tower>(new object[] { null, null, null });            
        }
    }

    public enum BehaviorState
    {
        MOCK,
        SPAWNING,
        IDLE,
        TARGETING,
        ATTACKING,
        RECOVERING
    }

    public TowerStats       stats           { get; private set; }
    public ITowerView       view            { get; private set; }
    public FP               spawnTime       { get; private set; }

    public BehaviorState    behaviorState   { get; set; }
    public Creep            targetCreep     { get; set; }
    public TowerState       state           { get; set; }


    private ITowerBrain _towerBrain;


	public Tower(
        ITowerBrain brain, 
        TowerStats pStat, 
        ITowerView pView)
    {
        _towerBrain = brain;

        spawnTime = TrueSyncManager.Time;

        view = pView;
        stats = pStat;
        state = TowerState.CreateFromStats(pStat);

        behaviorState = BehaviorState.SPAWNING;        
        view.tower = this;
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
        return new AttackData(
            stats.damageType, 
            state.attackDamage);
    }

    public AttackResult TakeDamage(AttackData attackData)
    {
        return default(AttackResult);
    }

    public void Step(float deltaTime)
    {

    }

    public void FixedStep(FP fixedDeltaTime)
    {
        if(_towerBrain != null)
        {
            _towerBrain.FixedStep(this, fixedDeltaTime);
        }
    }
}
