using UnityEngine;
using GhostGen;
using Zenject;

public enum YellowSignStateType
{
    NO_STATE = -1,

    INTRO = 1,
    MAIN_MENU,
    MULTIPLAYER_GAMEPLAY,
    MULTIPLAYER_GAME_SETUP,
    SINGLEPLAYER_GAMEPLAY,
    SINGLEPLAYER_GAME_SETUP,
    CREDITS
}


public class YellowSignStateFactory : ScriptableObjectInstaller, IStateFactory<YellowSignStateType>
{
    public override void InstallBindings()
    {
        Container.Bind<IntroState>().AsTransient();
        Container.Bind<MainMenuState>().AsTransient();
        Container.Bind<GamePlayState>().AsTransient();
    }

    public IGameState CreateState(YellowSignStateType stateId)
    {
        switch (stateId)
        {
            case YellowSignStateType.INTRO:                     return Container.Instantiate<IntroState>();
            case YellowSignStateType.MAIN_MENU:                 return Container.Instantiate<MainMenuState>();
            case YellowSignStateType.MULTIPLAYER_GAME_SETUP:    break;//return new GameplayState();
            case YellowSignStateType.MULTIPLAYER_GAMEPLAY:      return Container.Instantiate<GamePlayState>();//eturn new PlayerSetupState();
            case YellowSignStateType.SINGLEPLAYER_GAME_SETUP:   break;//return new MultiplayerSetupState();
            case YellowSignStateType.SINGLEPLAYER_GAMEPLAY:     break;//return new MultiplayerSetupState();
            case YellowSignStateType.CREDITS:                   break;
        }

        Debug.LogError("Error: state ID: " + stateId + " does not exist!");
        return null;
    }

}
