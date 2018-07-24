using System.Collections;
using System.Collections.Generic;
using Zenject;
using UnityEngine;

namespace GhostGen
{
	public class GameStateMachine<T> : ITickable
	{
		public GameStateMachine( IStateFactory<T> p_stateFactory )
		{
			_currentState 	= null;
            _currentId      = default(T);
			_stateFactory 	= p_stateFactory;
		}

		public void Tick()
		{
			if( _currentState != null )
				_currentState.Step( Time.deltaTime );
		}

		public void ChangeState( T stateId, Hashtable changeStateInfo = null )
		{
			if (EqualityComparer<T>.Default.Equals(_currentId, stateId))
                return;

			if( _currentState != null )
				_currentState.Exit( );

			_currentState = _stateFactory.CreateState( stateId );
			_currentState.Init(changeStateInfo);
		}
        
//------------------- Private Implementation -------------------
//--------------------------------------------------------------
		private IStateFactory<T> 	_stateFactory;
		private IGameState 		_currentState;
       
		private T _currentId;
	}
}