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
    private Tower.Factory _towerFactory;
    private GameplayResources _gameplayResources;
    private GuiManager _guiManager;

    public GamePlayState(
        [Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
        IEventDispatcher notificationDispatcher,
        CreepSystem creepSystem, 
        TowerSystem towerSystem, 
        Tower.Factory towerFactory, 
        GameplayResources gameplayResources,
        GuiManager guiManager)
    {
        _notificationDispatcher = notificationDispatcher;
        _creepSystem = creepSystem;
        _towerSystem = towerSystem;
        _towerFactory = towerFactory;
        _gameplayResources = gameplayResources;
        _guiManager = guiManager;
    }

    public void Init(Hashtable changeStateData)
    {
        //_notificationDispatcher = Singleton.instance.notificationDispatcher;
        _notificationDispatcher.AddListener(GameplayEventType.GAME_START, OnGameStart);

        PhotonNetwork.LoadLevel("GameScene");
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

        //SceneManager.UnloadSceneAsync("GameScene");

        TrueSyncManager.EndSimulation();
        TrueSyncManager.CleanUp();
    }

    private void OnGameStart(GhostGen.GeneralEvent e)
    {
        _notificationDispatcher.RemoveListener(GameplayEventType.GAME_START, OnGameStart);
        _playerList = GameObject.FindObjectsOfType<PlayerController>();
        PlayerSpawn[] spawnList = GameObject.FindObjectsOfType<PlayerSpawn>();
        List<PlayerSpawn> sp = new List<PlayerSpawn>(spawnList);
        sp.Sort((a, b) => a.playerNumber.CompareTo(b.playerNumber));

        for(int i = 0; i < _playerList.Length; ++i)
        {
            int spawnIndex = i < sp.Count ? i : sp.Count - 1;
            var spawnPoint = sp[spawnIndex];

            _playerList[i].Initialize(
                spawnPoint,
                _creepSystem, 
                _towerSystem, 
                _towerFactory, 
                _gameplayResources);
        }
        
        _guiManager.screenFader.FadeIn(1.5f);
    }
}
