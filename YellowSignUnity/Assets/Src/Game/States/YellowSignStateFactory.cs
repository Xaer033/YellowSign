using UnityEngine;
using GhostGen;
using Zenject;

public enum YellowSignState
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


public class YellowSignStateFactory : ScriptableObjectInstaller, IStateFactory<YellowSignState>
{
    public override void InstallBindings()
    {
        Container.Bind<IntroState>().AsTransient();
        Container.Bind<MainMenuState>().AsTransient();
        Container.Bind<GamePlayState>().AsTransient();
    }

    public IGameState CreateState(YellowSignState stateId)
    {
        switch (stateId)
        {
            case YellowSignState.INTRO:                     return Container.Instantiate<IntroState>();
            case YellowSignState.MAIN_MENU:                 return Container.Instantiate<MainMenuState>();
            case YellowSignState.MULTIPLAYER_GAME_SETUP:    break;//return new GameplayState();
            case YellowSignState.MULTIPLAYER_GAMEPLAY:      return Container.Instantiate<GamePlayState>();//eturn new PlayerSetupState();
            case YellowSignState.SINGLEPLAYER_GAME_SETUP:   break;//return new MultiplayerSetupState();
            case YellowSignState.SINGLEPLAYER_GAMEPLAY:     break;//return new MultiplayerSetupState();
            case YellowSignState.CREDITS:                   break;
        }

        Debug.LogError("Error: state ID: " + stateId + " does not exist!");
        return null;
    }

}
