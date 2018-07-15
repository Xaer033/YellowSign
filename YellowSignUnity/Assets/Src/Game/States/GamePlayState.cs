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

    [Inject]
    private CreepSystem _creepSystem;
    [Inject]
    private TowerSystem _towerSystem;
    [Inject]
    private TowerFactory _towerFactory;
    [Inject]
    private GameplayResources _gameplayResources;

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
            _playerList[i].Initialize();
        }

        Singleton.instance.gui.screenFader.FadeIn(1.5f);
    }
}
