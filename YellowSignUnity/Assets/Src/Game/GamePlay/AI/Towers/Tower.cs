using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;



public class Tower
{
    public TowerStats stats { get; private set; }
    public TowerState state { get; private set; }

    enum State
    {
        SPAWNING,
        IDLE,
        TARGETING,
        ATTACKING,
        RECOVERING
    }

    private State _currentState;
    private FP _spawnTime;
    private CreepSystem _creepSystem;


	public Tower(TowerStats pStat, CreepSystem creepSystem)
    {
        stats = pStat;
        _creepSystem = creepSystem;

        _currentState = State.SPAWNING;
        _spawnTime = TrueSyncManager.Time;
	}
	
    public void Step(float deltaTime)
    {

    }

    public void FixedStep(FP fixedDeltaTime)
    {
        switch(_currentState)
        {
            case State.SPAWNING:    _doSpawning();      break;
            case State.IDLE:        _doIdling();        break;
            case State.TARGETING:   _doTargeting();     break;
            case State.ATTACKING:   _doAttacking();     break;
            case State.RECOVERING:  _doRecovering();    break;
        }
    }
    
    private void _doSpawning()
    {

    }

    private void _doIdling()
    {

    }

    private void _doTargeting()
    {

    }

    private void _doAttacking()
    {

    }

    private void _doRecovering()
    {

    }

}
