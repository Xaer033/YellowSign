using GhostGen;
using System.Collections;
using Zenject;

public class LoadGameplayState : IGameState
{
    private GameStateMachine<YellowSignStateType> _gameStateMachine;
    private IEventDispatcher _notificationDispatcher;

    public LoadGameplayState(
        [Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
        IEventDispatcher notificationDispatcher,
        GameStateMachine<YellowSignStateType> gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
        _notificationDispatcher = notificationDispatcher;
    }

    public void Init(Hashtable changeStateData)
    {
        string sceneName = changeStateData["sceneName"] as string;
        _notificationDispatcher.AddListener(GameplayEventType.GAME_START, OnGameStart);
        PhotonNetwork.LoadLevel(sceneName);
    }

    public void Step(float p_deltaTime)
    {
        
    }

    public void Exit()
    {
        
    }

    private void OnGameStart(GeneralEvent e)
    {
        _notificationDispatcher.RemoveListener(GameplayEventType.GAME_START, OnGameStart);
        _gameStateMachine.ChangeState(YellowSignStateType.SINGLEPLAYER_GAMEPLAY);
    }
}
