using UnityEngine;
using System.Collections;
using GhostGen;
using DG.Tweening;

public class IntroState : IGameState
{
    private bool _gotoMainMenu = false;


    public void Init(Hashtable changeStateData)
	{
		Debug.Log ("Entering In Intro State");
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);

        _gotoMainMenu = true;
    }
    
    public void Step( float p_deltaTime )
	{
		if (_gotoMainMenu) 
		{
			Singleton.instance.gameStateMachine.ChangeState(YellowSignState.MAIN_MENU);
            _gotoMainMenu = false;
		}
    }

    public void Exit( )
	{
		Debug.Log ("Exiting In Intro State");
	}
    
}
