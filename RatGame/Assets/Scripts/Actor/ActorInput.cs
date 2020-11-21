using UnityEngine;

namespace Actor
{
	[RequireComponent(typeof(ActorBase))]
	public class ActorInput : MonoBehaviour
	{
		private GameControls _gameControls;
		
		private ActorBase _actorBase;
		
		private void Awake()
		{
			_gameControls = new GameControls();
			_actorBase = GetComponent<ActorBase>();
		}

		private void OnEnable()
		{
			
		}
	}
}
