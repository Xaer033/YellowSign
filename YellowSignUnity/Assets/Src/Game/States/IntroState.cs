using UnityEngine;
using GhostGen;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = System.Collections.Hashtable;

public class IntroState : IGameState
{
    private bool _gotoMainMenu = false;
    private GameStateMachine<YellowSignStateType> _gameStateMachine;
    private NetworkManager _networkManager;
    private GuiManager _gui;

    private int _playersTemp = 1;

    public IntroState(
        NetworkManager networkManager,
        GuiManager gui,
        GameStateMachine<YellowSignStateType> gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
        _networkManager = networkManager;
        _gui = gui;
    }
    
    public void Init(Hashtable changeStateData)
	{
		Debug.Log ("Entering In Intro State");
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
        ActorLayers.Init();
        
        _networkManager.Connect();
        _networkManager.onJoinedLobby += OnJoinedLobby;
        _networkManager.onJoinedRoom += OnJoinedRoom;
        _networkManager.onCustomEvent += OnCustomEvent;
    
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    public void Step( float p_deltaTime )
	{
		if (_gotoMainMenu) 
		{
            Hashtable table = new Hashtable();
            table["sceneName"] = (_playersTemp == 1) ? "GameSceneSingleplayer" : "GameSceneMultiplayer";
            _gameStateMachine.ChangeState(YellowSignStateType.LOAD_GAMEPLAY, table);
            _gotoMainMenu = false;
		}
    }

    public void Exit( )
	{
		Debug.Log ("Exiting In Intro State");

        _networkManager.onJoinedLobby -= OnJoinedLobby;
        _networkManager.onJoinedRoom -= OnJoinedRoom;
        _networkManager.onCustomEvent -= OnCustomEvent;
    }

    private void OnJoinedLobby()
    {
        RoomOptions options = new RoomOptions();
        PhotonNetwork.JoinOrCreateRoom("Poop", options, TypedLobby.Default);
    }
    
    void OnJoinedRoom()
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        Debug.Log("JOINED ROOM: Player count: " + PhotonNetwork.PlayerList.Length);
        if(PhotonNetwork.PlayerList.Length == _playersTemp)
        {
            PhotonNetwork.RaiseEvent(1, null, options, SendOptions.SendReliable);
        }
    }

    void OnCustomEvent(byte eventCode, object content, int senderId)
    {
        if(eventCode == 1)
        {
            Debug.Log("Fade Out Started");
            _gui.screenFader.FadeOut(0.50f, ()=>
            {
                _gotoMainMenu = true;
                Debug.Log("Fade Out Complete");
            });
        }
    }
}
