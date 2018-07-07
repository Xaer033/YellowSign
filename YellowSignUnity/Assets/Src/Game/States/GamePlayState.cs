using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using UnityEngine.SceneManagement;

public class GamePlayState : IGameState
{
    private PlayerController _playerController;
    private NotificationDispatcher _notificationDispatcher;

    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;

    public void Init(Hashtable changeStateData)
    {
        _creepSystem = new CreepSystem();
        _towerSystem = new TowerSystem();

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
        _playerController.CleanUp();

        //SceneManager.UnloadSceneAsync("GameScene");

        TrueSyncManager.EndSimulation();
        TrueSyncManager.CleanUp();
    }

    private void OnGameStart(GhostGen.Event e)
    {
        _notificationDispatcher.RemoveListener("GameStart", OnGameStart);
        PlayerController[] list = GameObject.FindObjectsOfType<PlayerController>();
        for(int i = 0; i < list.Length; ++i)
        {
            list[i].Setup(_creepSystem, _towerSystem);
            if (list[i].owner.Id == TrueSyncManager.LocalPlayer.Id)
            {
                _playerController = list[i];
            }
        }

        Singleton.instance.gui.screenFader.FadeIn(1.5f);
    }
}
