using UnityEngine;
using System.Collections;
using GhostGen;
using DG.Tweening;

public class MainMenuState : IGameState
{
    private MainMenuController _mainMenuController;

	public void Init( Hashtable changeStateInfo )
	{
		Debug.Log ("Entering In MainMenu State");

        _mainMenuController = new MainMenuController();
        _mainMenuController.Start();
        Singleton.instance.gui.screenFader.FadeIn();
    }
    
    public void Step( float p_deltaTime )
	{
		
    }

    public void Exit( )
	{
		Debug.Log ("Exiting In MainMenu State");

        _mainMenuController.RemoveView();
	}
    
}
