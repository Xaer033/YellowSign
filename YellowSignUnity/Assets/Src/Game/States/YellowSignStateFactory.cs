using UnityEngine;
using GhostGen;

public class YellowSignState
{
    public const int NO_STATE = -1;

    public const int INTRO = 1;
    public const int MAIN_MENU = 2;
    public const int MULTIPLAYER_GAMEPLAY = 3;
    public const int MULTIPLAYER_GAME_SETUP = 4;
    public const int SINGLEPLAYER_GAMEPLAY = 5;
    public const int SINGLEPLAYER_GAME_SETUP = 6;
    public const int CREDITS = 7;
}


public class YellowSignStateFactory : IStateFactory
{
    public IGameState CreateState(int stateId)
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
