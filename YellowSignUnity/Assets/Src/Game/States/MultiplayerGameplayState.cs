using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using UnityEngine.SceneManagement;
using Zenject;

public class MultiplayerGameplayState : IGameState
{
    private PlayerHudController _hudController;
    private PlayerController[] _playerList;

    private IEventDispatcher _notificationDispatcher;
    
    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;
    private GameplayResources _gameplayResources;
    private GuiManager _guiManager;
    private SyncStepper _commander;
    private MultiplayerGameSystemManager _gameSystemManager;
    //private SyncStepper.Factory _syncFactory;
   


    public MultiplayerGameplayState(
        [Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
        IEventDispatcher notificationDispatcher,
        MultiplayerGameSystemManager gameSystemManager,
        GameplayResources gameplayResources,
        GuiManager guiManager,
        SyncStepper.Factory syncFactory)
    {
        _notificationDispatcher = notificationDispatcher;
        _gameplayResources      = gameplayResources;       
        _guiManager             = guiManager;
        _gameSystemManager      = gameSystemManager;
    }

    public void Init(Hashtable changeStateData)    
    {
        _initialize();
    }

    private void _initialize()
    {
        _playerList = GameObject.FindObjectsOfType<PlayerController>();
        PlayerSpawn[] spawnList = GameObject.FindObjectsOfType<PlayerSpawn>();
        List<PlayerSpawn> sp = new List<PlayerSpawn>(spawnList);
        //sp.Sort((a, b) => a.playerNumber.CompareTo(b.playerNumber));

        byte localPlayerId = TrueSyncManager.LocalPlayer.Id;
        Debug.LogError("LocalPlayerID: " + localPlayerId);

        for(int i = 0; i < _playerList.Length; ++i)
        {
            int spawnIndex = i < sp.Count ? i : sp.Count - 1;
            var pController = _playerList[i];
            byte pControllerId = pController.owner.Id;

            Debug.LogError("PlayerControllerId: " + pControllerId);

            foreach(PlayerSpawn spawn in sp)
            {
                if(pControllerId == (byte)spawn.playerNumber)
                {
                    pController.playerSpawn = spawn;

                    Debug.LogError("Spawn Id: " + (byte)pController.playerSpawn.playerNumber);
                    break;
                }
            }
            
            pController.SetCurrentTower("basic_tower");
            
        }

        _gameSystemManager.Initialize();
        
        _guiManager.screenFader.FadeIn(0.5f);

        TrueSyncManager.RunSimulation();
        Debug.Log("Tick: " + TrueSyncManager.Ticks);
    }
    
    public void Step(float deltaTime)
    {
    }

    public void Exit()
    {
        for(int i = 0; i < _playerList.Length; ++i)
        {
            _playerList[i].CleanUp();
        }

        _gameSystemManager.CleanUp();
        
        TrueSyncManager.EndSimulation();
        TrueSyncManager.CleanUp();
    }
    
    //private void onFixedStep(FP fixedDeltaTime)
    //{
    //    if(_creepSystem != null)
    //    {
    //        _creepSystem.FixedStep(fixedDeltaTime);
    //    }

    //    if(_towerSystem != null)
    //    {
    //        _towerSystem.FixedStep(fixedDeltaTime);
    //    }
    //}
}
