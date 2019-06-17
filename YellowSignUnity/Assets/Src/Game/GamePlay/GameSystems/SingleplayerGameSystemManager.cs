using System.Collections;
using System.Collections.Generic;
using GhostGen;
using TrueSync;
using UnityEngine;
using Zenject;


public class SingleplayerGameSystemManager : EventDispatcher
{
    private GameState _gameState;
    private TowerSystem _towerSystem;
    private CreepSystem _creepSystem;
    private CreepHealthUISystem _creepHealthUISystem;
    private CreepViewSystem _creepViewSystem;
    private WaveSpawnerSystem _waveSpawnerSystem;
    private SyncStepper _syncStepper;
    private SyncStepper.Factory _syncFactory;
    private GameplayResources _gameplayResources;
    
    [Inject]
    private NetworkManager _networkManager;

//    [Inject]
    private WaveAISystem _waveAISystem;

    [Inject]
    private GameTimerManager _timerManager;

    private Dictionary<int, Dictionary<int, string>> _checksumMap;

    public SingleplayerGameSystemManager(
        GameState gameState,
        TowerSystem towerSystem,
        CreepSystem creepSystem,
        CreepViewSystem creepViewSystem,
        CreepHealthUISystem creepHealthUISystem,
        WaveSpawnerSystem waveSpawnerSystem,
        WaveAISystem waveAISystem,
        GameplayResources gameplayResources,
        SyncStepper.Factory syncFactory)
    {
        _gameState = gameState;
        _towerSystem = towerSystem;
        _creepSystem = creepSystem;
        _creepViewSystem = creepViewSystem;
        _creepHealthUISystem = creepHealthUISystem;
        _waveSpawnerSystem = waveSpawnerSystem;
        _waveAISystem = waveAISystem;
        _syncFactory = syncFactory;
        _gameplayResources = gameplayResources;

        _checksumMap = new Dictionary<int, Dictionary<int, string>>();

    }

    public void Initialize()
    {
        _syncStepper = _syncFactory.Create();
        _syncStepper.onSyncStart += onSyncStart;
        _syncStepper.onSyncedStep += onSyncStep;
        _syncStepper.onFrameUpdate += onFrameUpdate;

        _towerSystem.AddListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);
        _creepSystem.AddListener(GameplayEventType.CREEP_REACHED_GOAL, onCreepGoalReached);
        _creepSystem.AddListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);
        _creepSystem.AddListener(GameplayEventType.CREEP_KILLED, onCreepKilled);
        _creepSystem.AddListener(GameplayEventType.CREEP_SPAWNED, onCreepSpawned);
        _waveSpawnerSystem.AddListener(GameplayEventType.WAVE_START, onWaveStart);
        _waveSpawnerSystem.AddListener(GameplayEventType.WAVE_COMPLETE, onWaveComplete);
        
    }

    public void CleanUp()
    {
        _syncStepper.onSyncStart -= onSyncStart;
        _syncStepper.onSyncedStep -= onSyncStep;
        _syncStepper.onFrameUpdate -= onFrameUpdate;

        _towerSystem.RemoveListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_REACHED_GOAL, onCreepGoalReached);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_KILLED, onCreepKilled);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_SPAWNED, onCreepSpawned);
        _waveSpawnerSystem.RemoveListener(GameplayEventType.WAVE_START, onWaveStart);
        _waveSpawnerSystem.RemoveListener(GameplayEventType.WAVE_COMPLETE, onWaveComplete);

        _waveAISystem.CleanUp();
    }

    private void onSyncStart()
    {
        _waveAISystem.Start(_gameplayResources.waveSequence);
    }
    
    private void onSyncStep(FP fixedDeltaTime)
    {
        _creepSystem.FixedStep(fixedDeltaTime);
        _towerSystem.FixedStep(fixedDeltaTime);
        _waveSpawnerSystem.FixedStep(fixedDeltaTime);
        _waveAISystem.FixedStep(fixedDeltaTime);
    }

    private void onFrameUpdate(float deltaTime)
    {
        _creepSystem.Step(deltaTime);
        _towerSystem.Step(deltaTime);

        _creepViewSystem.Step(deltaTime);
    }

    private void onTowerBuilt(GeneralEvent e)
    {
        _creepSystem.recalculatePaths = true;
    }


    private void onCreepGoalReached(GhostGen.GeneralEvent e)
    {
        Creep c = e.data as Creep;
        //Debug.Log("Creep Goal reached: " + c.targetOwnerId + " loses life" );
    }

    private void onCreepKilled(GhostGen.GeneralEvent e)
    {

        Creep c = e.data as Creep;
        //Debug.Log("Creep Death: " + c.ownerId + "'s creep died");
        _creepHealthUISystem.RemoveCreep(c);
    }

    private void onCreepSpawned(GhostGen.GeneralEvent e)
    {

        Creep c = e.data as Creep;
        //Debug.Log("Creep Spawned: " + c.stats.creepType.ToString() + " on playfield");
        _creepViewSystem.AddCreep(c);
        _creepHealthUISystem.AddCreep(c);
    }

    private void onCreepDamaged(GhostGen.GeneralEvent e)
    {

        AttackResult ar = (AttackResult)e.data;
        _creepHealthUISystem.ShowHealthOnCreep(ar.target as Creep);
        //Debug.LogFormat("Creep took '{0}' and has '{1}' remaining health!", ar.totalDamageDelt, ar.targetHealthRemaining);
    }

    private void onWaveStart(GhostGen.GeneralEvent e)
    {
        Debug.Log("Wave start: " + e.data);
    }

    private void onWaveComplete(GhostGen.GeneralEvent e)
    {

        Debug.Log("Wave complete: " + e.data);
    }
}
