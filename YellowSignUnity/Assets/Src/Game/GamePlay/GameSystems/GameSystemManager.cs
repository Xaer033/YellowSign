
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GhostGen;
using Zenject;
using TrueSync;
using ExitGames.Client.Photon;

public class GameSystemManager : EventDispatcher
{
    private GameState _gameState;
    private TowerSystem _towerSystem;
    private CreepSystem _creepSystem;
    private CreepHealthUISystem _creepHealthUISystem;
    private CreepViewSystem _creepViewSystem;
    private WaveSpawnerSystem _waveSpawnerSystem;
    private SyncStepper _syncStepper;
    private SyncStepper.Factory _syncFactory;

    [Inject]
    private NetworkManager _networkManager;

    [Inject]
    private WaveAISystem _waveAISystem;

    [Inject]
    private GameTimerManager _timerManager;

    private Dictionary<int, Dictionary<int, string>> _checksumMap;

    public GameSystemManager(
        GameState gameState,
        TowerSystem towerSystem,
        CreepSystem creepSystem,
        CreepViewSystem creepViewSystem,
        CreepHealthUISystem creepHealthUISystem,
        WaveSpawnerSystem waveSpawnerSystem,
        SyncStepper.Factory syncFactory)
    {
        _gameState = gameState;
        _towerSystem = towerSystem;
        _creepSystem = creepSystem;
        _creepViewSystem = creepViewSystem;
        _creepHealthUISystem = creepHealthUISystem;
        _waveSpawnerSystem = waveSpawnerSystem;
        _syncFactory = syncFactory;

        _checksumMap = new Dictionary<int, Dictionary<int, string>>();

    }

    public void Initialize()
    {
        _syncStepper = _syncFactory.Create();
        _syncStepper.onSyncedStep += onSyncStep;
        _syncStepper.onFrameUpdate += onFrameUpdate;

        _towerSystem.AddListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);
        _creepSystem.AddListener(GameplayEventType.CREEP_REACHED_GOAL, onCreepGoalReached);
        _creepSystem.AddListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);
        _creepSystem.AddListener(GameplayEventType.CREEP_KILLED, onCreepKilled);
        _creepSystem.AddListener(GameplayEventType.CREEP_SPAWNED, onCreepSpawned);
        _waveSpawnerSystem.AddListener(GameplayEventType.WAVE_START, onWaveStart);
        _waveSpawnerSystem.AddListener(GameplayEventType.WAVE_COMPLETE, onWaveComplete);
        
        _networkManager.onCustomEvent += onCustomEvent;
    }

    public void CleanUp()
    {
        _syncStepper.onSyncedStep -= onSyncStep;
        _syncStepper.onFrameUpdate -= onFrameUpdate;

        _towerSystem.RemoveListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_REACHED_GOAL, onCreepGoalReached);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_KILLED, onCreepKilled);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_SPAWNED, onCreepSpawned);
        _waveSpawnerSystem.RemoveListener(GameplayEventType.WAVE_START, onWaveStart);
        _waveSpawnerSystem.RemoveListener(GameplayEventType.WAVE_COMPLETE, onWaveComplete);

        _networkManager.onCustomEvent -= onCustomEvent;

        _waveAISystem.CleanUp();
    }


    private void onSyncStep(FP fixedDeltaTime)
    {
        _creepSystem.FixedStep(fixedDeltaTime);
        _towerSystem.FixedStep(fixedDeltaTime);
        _waveSpawnerSystem.FixedStep(fixedDeltaTime);
        
        if(TrueSyncManager.Players.Count >= 2)
        {
            md5Checksum();
        }
    }

    private void onFrameUpdate(float deltaTime)
    {
        _creepSystem.Step(deltaTime);
        _towerSystem.Step(deltaTime);

        _creepViewSystem.Step(deltaTime);
    }

    private void md5Checksum()
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        Hashtable table = new Hashtable();
        table["tick"] = TrueSyncManager.Ticks;
        table["id"] = PhotonNetwork.player.ID;
        table["checksum"] = _gameState.GetMd5();
        PhotonNetwork.RaiseEvent(2, table, true, options);
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

    private void onCustomEvent(byte eventCode, object content, int senderId)
    {
        if(eventCode == 2)
        {
            Hashtable c = content as Hashtable;
            int tick = (int)c["tick"];
            int id = (int)c["id"];            
            string checksum = c["checksum"] as string;

            addChecksum(tick, id, checksum);
        }
    }

    private bool addChecksum(int tick, int id, string checksum)
    {
        bool result = false;
        Dictionary<int, string> playerMap;
        if(!_checksumMap.TryGetValue(tick, out playerMap))
        {
            playerMap = new Dictionary<int, string>(NetworkManager.kMaxPlayers);
            _checksumMap.Add(tick, playerMap);
        }

        playerMap[id] = checksum;

        if(playerMap.Count == TrueSyncManager.Players.Count)
        {
            string firstHash = playerMap[1];
            foreach(var pair in playerMap)
            {
                if(!StateChecker.VerifyMd5Hash(firstHash, pair.Value))
                {
                    Debug.LogError("Checksum error!");
                    //EditorApplication.isPaused = true;
                }
                else
                {
                    result = true;
                }
            }
        }
        return result;
    }
}
