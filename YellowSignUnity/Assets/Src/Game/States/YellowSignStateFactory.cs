using UnityEngine;
using GhostGen;
using Zenject;

public enum YellowSignState
{
    NO_STATE = -1,

    INTRO = 1,
    MAIN_MENU = 2,
    MULTIPLAYER_GAMEPLAY = 3,
    MULTIPLAYER_GAME_SETUP = 4,
    SINGLEPLAYER_GAMEPLAY = 5,
    SINGLEPLAYER_GAME_SETUP = 6,
    CREDITS = 7
}


public class YellowSignStateFactory : Installer, IStateFactory<YellowSignState>
{
    public override void InstallBindings()
    {
        Container.Bind<IGameState>().FromMethod<YellowSignState>(CreateState).AsTransient();
    }

    public IGameState CreateState(YellowSignState stateId, InjectContext ctx)
    {
        switch (stateId)
        {
            case YellowSignState.INTRO:                     return new IntroState();
            case YellowSignState.MAIN_MENU:                 return new MainMenuState();
            case YellowSignState.MULTIPLAYER_GAME_SETUP:    break;//eturn new GameplayState();
            case YellowSignState.MULTIPLAYER_GAMEPLAY:      return new GamePlayState();//eturn new PlayerSetupState();
            case YellowSignState.SINGLEPLAYER_GAME_SETUP:   break;//eturn new MultiplayerSetupState();
            case YellowSignState.SINGLEPLAYER_GAMEPLAY:     break;//return new MultiplayerSetupState();
            case YellowSignState.CREDITS:    break;
        }

        Debug.LogError("Error: state ID: " + stateId + " does not exist!");
        return null;
    }
    
}
