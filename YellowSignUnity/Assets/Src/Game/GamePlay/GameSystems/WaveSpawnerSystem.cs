using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;

public class WaveSpawnerSystem : EventDispatcher
{
    public enum State
    {
        IDLE,
        SPAWNING,
        SPAWN_PAUSE
    }

    private CreepSystem _creepSystem;
    private FP _spawnTimer;
    private State _spawnState;
    private SpawnWaveInfo _currentWave;
    private int _currentCreepCount;

    private Queue<SpawnWaveInfo> _spawnQueue = new Queue<SpawnWaveInfo>();

    public TSVector spawnPoint { get; set; }


    public WaveSpawnerSystem(CreepSystem creepSystem)
    {
        _creepSystem = creepSystem;
        _spawnState = State.IDLE;
    }

    public void AddWave(SpawnWaveInfo spawnWave)
    {
        _spawnQueue.Enqueue(spawnWave);
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        switch(_spawnState)
        {
            case State.IDLE:        _idle(fixedDeltaTime);       break;
            case State.SPAWNING:    _spawning(fixedDeltaTime);   break;
            case State.SPAWN_PAUSE: _spawnPause(fixedDeltaTime); break;
        }
    }

    private void _spawning(FP fixedDeltaTime)
    {

        string creepId = _currentWave.spawnCommand.type;
        CreepSpawnInfo spawnInfo = _currentWave.spawnInfo;

        Creep c = _creepSystem.AddCreep(creepId, spawnInfo);
        FP scaler = 0.2;
        _spawnTimer = _currentWave.betweenSpawnDelay * c.stats.baseSpeed * scaler;
        
        _currentCreepCount--;
        
        _spawnState = State.SPAWN_PAUSE;
    }

    private void _idle(FP fixedDeltaTime)
    {
        if(_spawnQueue.Count > 0)
        {
            _spawnState = State.SPAWNING;
            _currentWave = _spawnQueue.Dequeue();
            _currentCreepCount = _currentWave.spawnCommand.count;

            DispatchEvent(GameplayEventType.WAVE_START, true, _currentWave);
        }
    }

    private void _spawnPause(FP fixedDeltaTime)
    {
        _spawnTimer -= fixedDeltaTime;

        if(_spawnTimer <= 0)
        {
            if(_currentCreepCount > 0)
            {
                _spawnState = State.SPAWNING;
            }
            else
            {
                _spawnState = State.IDLE;
                DispatchEvent(GameplayEventType.WAVE_COMPLETE, true, _currentWave);
            }
        }
    }
}
