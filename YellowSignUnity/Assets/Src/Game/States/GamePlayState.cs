using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using UnityEngine.SceneManagement;

public class GamePlayState : IGameState
{
    private Commander _playerCommander;

    public void Init(Hashtable changeStateData)
    {
        //TrueSyncManager.RunSimulation();
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

        SceneManager.UnloadSceneAsync("GameScene");

        TrueSyncManager.EndSimulation();
        TrueSyncManager.CleanUp();
    }
}
