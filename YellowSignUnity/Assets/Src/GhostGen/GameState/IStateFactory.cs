using UnityEngine;
using System.Collections;


namespace GhostGen
{
	public interface IStateFactory<T>
    {
		IGameState CreateState( T stateId );
	}
}