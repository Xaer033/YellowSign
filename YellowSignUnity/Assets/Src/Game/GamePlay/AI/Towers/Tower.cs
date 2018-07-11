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
