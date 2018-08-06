using TrueSync;
using UnityEngine;
using Zenject;


public class Tower : IAttacker, IAttackTarget
{ 
    public class Factory : PlaceholderFactory<string, TowerSpawnInfo, Tower>, IValidatable
    {
        private DiContainer _container;
        private TowerDictionary _towerDefs;

        public Factory(
            TowerDictionary towerDefs, 
            DiContainer container)
        {
            _container = container;
            _towerDefs = towerDefs;
        }

        public override Tower Create(string towerId, TowerSpawnInfo spawnInfo)
        {
            TowerDef def = _towerDefs.GetDef(towerId) as TowerDef;
            Tower tower = _container.Instantiate<Tower>(new object[] { def, spawnInfo });
            return tower;
        }

        public override void Validate()
        {
            //TowerDef def = _towerDefs.GetDef("basic_tower");
            _container.Instantiate<Tower>(new object[] { null, null });            
        }
    }

    public enum BehaviorState
    {
        MOCK,
        SPAWNING,
        IDLE,
        TARGETING,
        VISUAL_ATTACK,
        ATTACK,
        RECOVERING
    }

    public byte             ownerId         { get; private set; }
    public TowerStats       stats           { get; private set; }
    public ITowerView       view            { get; private set; }
    public FP               spawnTime       { get; private set; }

    public BehaviorState    behaviorState   { get; set; }
    public Creep            targetCreep     { get; set; }
    public TowerState       state           { get; set; }


    private ITowerBrain _towerBrain;


	public Tower(TowerDef def, TowerSpawnInfo spawnInfo)
    {
        ownerId = spawnInfo.ownerId;
        spawnTime = TrueSyncManager.Time;

        _towerBrain = def.brain;
        
        stats = def.stats;
        state = TowerState.CreateFromStats(stats);

        GameObject towerGameObject = TrueSyncManager.SyncedInstantiate(
            def.view.gameObject, 
            spawnInfo.position, 
            spawnInfo.rotation);

        view = towerGameObject.GetComponent<ITowerView>();
        view.tower = this;

        behaviorState = BehaviorState.SPAWNING;        
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
