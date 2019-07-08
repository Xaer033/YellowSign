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
            Tower result = null;
            TowerDef def = _towerDefs.GetDef(towerId) as TowerDef;
            if (def != null)
            {
                result = _container.Instantiate<Tower>(new object[] { def, spawnInfo });
            }
            return result;
        }

        public override void Validate()
        {
            //TowerDef def = _towerDefs.GetDef("basic_tower");
            _container.Instantiate<Tower>(new object[] { null, null });            
        }
    }


    public byte             ownerId         { get; private set; }
    public string           type            { get; private set; }
    public TowerStats       stats           { get; private set; }
    public ITowerView       view            { get; private set; }
    public FP               spawnTime       { get; private set; }
    public TowerDef         def             { get; private set; }

    public Creep            targetCreep     { get; set; }
    public TowerState       state           { get; set; }
    
    
    private ITowerBrain _towerBrain;


	public Tower(TowerDef towerDef, TowerSpawnInfo spawnInfo)
    {
        ownerId = spawnInfo.ownerId;
        spawnTime = TrueSyncManager.Time;

        def = towerDef;
        
        _towerBrain = towerDef.brain;

        type = towerDef.id;
        
        stats = towerDef.stats;
        state = TowerState.CreateFromStats(stats);

        GameObject towerGameObject = TrueSyncManager.SyncedInstantiate(
            towerDef.view.gameObject, 
            spawnInfo.position, 
            spawnInfo.rotation);

        view = towerGameObject.GetComponent<ITowerView>();
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
