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

    public void Init(Hashtable changeStateData)
    {
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

        SceneManager.UnloadSceneAsync("GameScene");

        TrueSyncManager.EndSimulation();
        TrueSyncManager.CleanUp();
    }

    private void OnGameStart(Hashtable info)
    {
        _notificationDispatcher.RemoveListener("GameStart", OnGameStart);
        PlayerController[] list = GameObject.FindObjectsOfType<PlayerController>();
        for(int i = 0; i < list.Length; ++i)
        {
            if (list[i].owner.Id == TrueSyncManager.LocalPlayer.Id)
            {
                _playerController = list[i];
                break;
            }
        }

        Singleton.instance.gui.screenFader.FadeIn(1.5f);
    }
}
