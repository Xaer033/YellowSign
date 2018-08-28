using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using UnityEngine.SceneManagement;
using Zenject;

public class SingleplayerGameplayState : IGameState
{
    private PlayerHudController _hudController;
    private PlayerController _playerController;

    private IEventDispatcher _notificationDispatcher;
    
    private GameplayResources _gameplayResources;
    private GuiManager _guiManager;
    private SyncStepper _commander;
    private GameSystemManager _gameSystems;
    //private SyncStepper.Factory _syncFactory;
   
    public SingleplayerGameplayState(
        [Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
        IEventDispatcher notificationDispatcher,
        GameSystemManager gameSystems,
        GameplayResources gameplayResources,
        GuiManager guiManager,
        SyncStepper.Factory syncFactory)
    {
        _notificationDispatcher = notificationDispatcher;
        _gameplayResources      = gameplayResources;       
        _guiManager     = guiManager;
        _gameSystems    = gameSystems;
    }

    public void Init(Hashtable changeStateData)
    {
        _initialize();
    }

    private void _initialize()
    {
        PlayerSpawn playerSpawn = GameObject.FindObjectOfType<PlayerSpawn>();
        _playerController = GameObject.FindObjectOfType<PlayerController>();

        _playerController.playerSpawn = playerSpawn;
        _playerController.SetCurrentTower("basic_tower");
        
        _gameSystems.Initialize();

        _hudController = new PlayerHudController(_playerController);
        _hudController.Start(() =>
        {
            _guiManager.screenFader.FadeIn(1.5f);
        });

        Debug.Log("Tick: " + TrueSyncManager.Ticks);
        TrueSyncManager.RunSimulation();
    }
    
    public void Step(float deltaTime)
    {
    }

    public void Exit()
    {
        _playerController.CleanUp();
        _gameSystems.CleanUp();
        
        TrueSyncManager.EndSimulation();
        TrueSyncManager.CleanUp();
    }
}
