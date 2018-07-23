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
    private EventDispatcher _notificationDispatcher;
    
    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;
    private Tower.Factory _towerFactory;
    private GameplayResources _gameplayResources;

    public GamePlayState(
        CreepSystem creepSystem, 
        TowerSystem towerSystem, 
        Tower.Factory towerFactory, 
        GameplayResources gameplayResources)
    {
        _creepSystem = creepSystem;
        _towerSystem = towerSystem;
        _towerFactory = towerFactory;
        _gameplayResources = gameplayResources;
    }

    public void Init(Hashtable changeStateData)
    {
        //_creepSystem = new CreepSystem();
        //_towerSystem = new TowerSystem();

        //TrueSyncManager.RunSimulation();
        _notificationDispatcher = Singleton.instance.notificationDispatcher;
        _notificationDispatcher.AddListener("GameStart", OnGameStart);

        PhotonNetwork.LoadLevel("GameScene");
        

        //if (PhotonNetwork.isMasterClient)
        //{
        //    PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced("GameScene");

        //    PhotonNetwork.isMessageQueueRunning = false;
        //    PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
        //    SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
        //}
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
        _notificationDispatcher.RemoveListener("GameStart", OnGameStart);
        _playerList = GameObject.FindObjectsOfType<PlayerController>();

        for(int i = 0; i < _playerList.Length; ++i)
        {
            _playerList[i].Initialize(
                _creepSystem, 
                _towerSystem, 
                _towerFactory, 
                _gameplayResources);
        }

        Singleton.instance.gui.screenFader.FadeIn(1.5f);
    }
}
