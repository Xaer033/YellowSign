using UnityEngine;
using System.Collections;
using GhostGen;
using DG.Tweening;

public class IntroState : IGameState
{
    private bool _gotoMainMenu = false;
    private GameStateMachine<YellowSignStateType> _gameStateMachine;
    private NetworkManager _networkManager;
    private GuiManager _gui;

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

        _networkManager.Connect();
        _networkManager.onJoinedLobby += OnJoinedLobby;
        _networkManager.onJoinedRoom += OnJoinedRoom;
        _networkManager.onCustomEvent += OnCustomEvent;
    
        PhotonNetwork.automaticallySyncScene = true;
    }
    
    public void Step( float p_deltaTime )
	{
		if (_gotoMainMenu) 
		{
            _gameStateMachine.ChangeState(YellowSignStateType.MULTIPLAYER_GAMEPLAY);
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

    void OnJoinedLobby()
    {
        RoomOptions options = new RoomOptions();
        PhotonNetwork.JoinOrCreateRoom("Poop", options, TypedLobby.Default);
    }
    
    void OnJoinedRoom()
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        Debug.Log("JOINED ROOM: Player count: " + PhotonNetwork.playerList.Length);
        if(PhotonNetwork.playerList.Length == 1)
        {
            PhotonNetwork.RaiseEvent(1, null, true, options);
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
