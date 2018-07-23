using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Zenject;


public class Tower
{
    public class Factory : PlaceholderFactory<string, TSVector, TSQuaternion, Tower>
    {

    }

    public class DynamicFactory : IFactory<string, TSVector, TSQuaternion, Tower>, IValidatable
    {
        private DiContainer _container;
        private TowerDictionary _towerDefs;

        public DynamicFactory(TowerDictionary towerDefs, DiContainer container)
        {
            _container = container;
            _towerDefs = towerDefs;
        }

        public Tower Create(string towerId, TSVector position, TSQuaternion rotation)
        {
            TowerDef def = _towerDefs.GetDef(towerId);
            GameObject towerGameObject = TrueSyncManager.SyncedInstantiate(def.view.gameObject, position, rotation);
            ITowerView towerView = towerGameObject.GetComponent<ITowerView>();

            Tower tower = _container.Instantiate<Tower>(new object[] { def.stats, towerView, def.brain });
            return tower;
        }

        public void Validate()
        {
            TowerDef def = _towerDefs.GetDef("basic_tower");
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

    public TowerStats stats { get; private set; }
    public TowerState state { get; private set; }
    public ITowerView view  { get; private set; }

    public BehaviorState behaviorState { get; set; }

    private FP _spawnTime;
    private CreepSystem _creepSystem;
    private ITowerBrain _towerBrain;

	public Tower(ITowerBrain brain, TowerStats pStat, ITowerView pView, CreepSystem creepSystem)
    {
        view = pView;
        stats = pStat;
        state = TowerState.CreateFromStats(stats);

        behaviorState = BehaviorState.SPAWNING;

        _towerBrain = brain;
        _creepSystem = creepSystem;

        _spawnTime = TrueSyncManager.Time;
	}
	
    public void Step(float deltaTime)
    {

    }

    public void FixedStep(FP fixedDeltaTime)
    {
        if(_towerBrain != null)
        {
            _towerBrain.FixedStep(this, fixedDeltaTime, _creepSystem);
        }
    }
}
