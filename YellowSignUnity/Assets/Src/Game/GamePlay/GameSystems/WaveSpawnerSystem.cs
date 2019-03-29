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

    private class PlayerSpawnWaveInfo
    {
        public FP spawnTimer;
        public State spawnState;
        public SpawnWaveInfo currentWave;
        public int currentCreepCount;
        public Queue<SpawnWaveInfo> spawnQueue = new Queue<SpawnWaveInfo>();
    }

    private CreepSystem _creepSystem;
    //private FP _spawnTimer;
    //private State _spawnState;
    //private SpawnWaveInfo _currentWave;
    //private int _currentCreepCount;

    private Dictionary<PlayerNumber, PlayerSpawnWaveInfo> _playerSpawnQueue = new Dictionary<PlayerNumber, PlayerSpawnWaveInfo>();
    //private Queue<SpawnWaveInfo> _spawnQueue = new Queue<SpawnWaveInfo>();

    public TSVector spawnPoint { get; set; }


    public WaveSpawnerSystem(CreepSystem creepSystem)
    {
        _creepSystem = creepSystem;
        //_spawnState = State.IDLE;
    }

    public void AddWave(SpawnWaveInfo spawnWave)
    {
        //_spawnQueue.Enqueue(spawnWave);
        PlayerSpawnWaveInfo playerSpawnWaveInfo;
        PlayerNumber pNumber = (PlayerNumber)spawnWave.spawnInfo.targetOwnerId;
        if(!_playerSpawnQueue.TryGetValue(pNumber, out playerSpawnWaveInfo))
        {
            playerSpawnWaveInfo = new PlayerSpawnWaveInfo();
            playerSpawnWaveInfo.spawnState = State.IDLE;
            _playerSpawnQueue.Add(pNumber, playerSpawnWaveInfo);
            Debug.Log("Creating new info for player: " + pNumber);
        }

        playerSpawnWaveInfo.spawnQueue.Enqueue(spawnWave);
        Debug.Log("Spawning for target player: " + pNumber);
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        foreach(var pair in _playerSpawnQueue)
        {
            PlayerSpawnWaveInfo playerSpawnWaveInfo = pair.Value;
            State state = playerSpawnWaveInfo.spawnState;
            switch(state)
            {
                case State.IDLE:        _idle(fixedDeltaTime, playerSpawnWaveInfo);       break;
                case State.SPAWNING:    _spawning(fixedDeltaTime, playerSpawnWaveInfo);   break;
                case State.SPAWN_PAUSE: _spawnPause(fixedDeltaTime, playerSpawnWaveInfo); break;
            }
        }
    }

    private void _spawning(FP fixedDeltaTime, PlayerSpawnWaveInfo info)
    {
        string creepId = info.currentWave.spawnCommand.type;
        CreepSpawnInfo spawnInfo = info.currentWave.spawnInfo;

        Creep c = _creepSystem.AddCreep(creepId, spawnInfo);
        FP scaler = 0.2;
        info.spawnTimer = info.currentWave.betweenSpawnDelay * c.stats.baseSpeed * scaler;

        info.currentCreepCount--;

        info.spawnState = State.SPAWN_PAUSE;
    }

    private void _idle(FP fixedDeltaTime, PlayerSpawnWaveInfo info)
    {
        if(info.spawnQueue.Count > 0)
        {
            info.spawnState = State.SPAWNING;
            info.currentWave = info.spawnQueue.Dequeue();
            info.currentCreepCount = info.currentWave.spawnCommand.count;

            DispatchEvent(GameplayEventType.WAVE_START, true, info.currentWave);
        }
    }

    private void _spawnPause(FP fixedDeltaTime, PlayerSpawnWaveInfo info)
    {
        info.spawnTimer -= fixedDeltaTime;

        if(info.spawnTimer <= 0)
        {
            if(info.currentCreepCount > 0)
            {
                info.spawnState = State.SPAWNING;
            }
            else
            {
                info.spawnState = State.IDLE;
                DispatchEvent(GameplayEventType.WAVE_COMPLETE, true, info.currentWave);
            }
        }
    }
}
