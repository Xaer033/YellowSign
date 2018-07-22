using UnityEngine;
using System.Collections;
using Zenject;

namespace GhostGen
{
	public interface IStateFactory<T>
    {
		IGameState CreateState( T stateId );
	}
}