using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;



public class Tower
{
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
    public GameObject view  { get; private set; }

    public BehaviorState behaviorState { get; set; }

    private FP _spawnTime;
    private CreepSystem _creepSystem;
    private ITowerBrain _towerBrain;

	public Tower(ITowerBrain brain, TowerStats pStat, GameObject view, CreepSystem creepSystem)
    {
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
