using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using UnityEngine.SceneManagement;
using Zenject;

public class GamePlayState : IGameState
{
    private PlayerController[] _playerList;

    private IEventDispatcher _notificationDispatcher;
    
    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;
    private GameplayResources _gameplayResources;
    private GuiManager _guiManager;
    private SyncStepper _commander;
    private GameSystemManager _gameSystems;
    //private SyncStepper.Factory _syncFactory;


    public GamePlayState(
        [Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
        IEventDispatcher notificationDispatcher,
        GameSystemManager gameSystems,
        CreepSystem creepSystem,
        TowerSystem towerSystem, 
        GameplayResources gameplayResources,
        GuiManager guiManager,
        SyncStepper.Factory syncFactory)
    {
        _notificationDispatcher = notificationDispatcher;
        _gameplayResources      = gameplayResources;
        _creepSystem    = creepSystem;
        _towerSystem    = towerSystem;        
        _guiManager     = guiManager;
        _gameSystems    = gameSystems;
    }

    public void Init(Hashtable changeStateData)
    {
        //_notificationDispatcher = Singleton.instance.notificationDispatcher;
        _notificationDispatcher.AddListener(GameplayEventType.GAME_START, OnGameStart);

        PhotonNetwork.LoadLevel("GameScene");
    }

    private void OnGameStart(GhostGen.GeneralEvent e)
    {
        _notificationDispatcher.RemoveListener(GameplayEventType.GAME_START, OnGameStart);
        
        _playerList = GameObject.FindObjectsOfType<PlayerController>();
        PlayerSpawn[] spawnList = GameObject.FindObjectsOfType<PlayerSpawn>();
        List<PlayerSpawn> sp = new List<PlayerSpawn>(spawnList);
        sp.Sort((a, b) => a.playerNumber.CompareTo(b.playerNumber));

        Debug.Log("CreepSystem: " + _creepSystem.GetHashCode());

        for(int i = 0; i < _playerList.Length; ++i)
        {
            int spawnIndex = i < sp.Count ? i : sp.Count - 1;
            var spawnPoint = sp[spawnIndex];

            _playerList[i].Initialize(
                spawnPoint,
                _creepSystem,
                _towerSystem,
                _gameplayResources);
        }

        _gameSystems.Initialize();
        
        _guiManager.screenFader.FadeIn(1.5f);
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

        _gameSystems.CleanUp();
        
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
