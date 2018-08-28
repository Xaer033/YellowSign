using UnityEngine;
using GhostGen;
using Zenject;

public enum YellowSignStateType
{
    NO_STATE = -1,

    INTRO = 1,
    MAIN_MENU,
    LOAD_GAMEPLAY,
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
        Container.Bind<SingleplayerGameplayState>().AsTransient();
        Container.Bind<MultiplayerGameplayState>().AsTransient();
        Container.Bind<LoadGameplayState>().AsTransient();
    }

    public IGameState CreateState(YellowSignStateType stateId)
    {
        switch (stateId)
        {
            case YellowSignStateType.INTRO:                     return Container.Resolve<IntroState>();
            case YellowSignStateType.MAIN_MENU:                 return Container.Resolve<MainMenuState>();
            case YellowSignStateType.LOAD_GAMEPLAY:             return Container.Resolve<LoadGameplayState>();
            case YellowSignStateType.MULTIPLAYER_GAME_SETUP:    break;//return new GameplayState();
            case YellowSignStateType.MULTIPLAYER_GAMEPLAY:      return Container.Resolve<MultiplayerGameplayState>();//eturn new PlayerSetupState();
            case YellowSignStateType.SINGLEPLAYER_GAME_SETUP:   break;//return new MultiplayerSetupState();
            case YellowSignStateType.SINGLEPLAYER_GAMEPLAY:     return Container.Resolve<SingleplayerGameplayState>();
            case YellowSignStateType.CREDITS:                   break;
        }

        Debug.LogError("Error: state ID: " + stateId + " does not exist!");
        return null;
    }
}
