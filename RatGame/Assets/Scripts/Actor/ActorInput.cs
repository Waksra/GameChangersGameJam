using UnityEngine;
using UnityEngine.InputSystem;

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
			_gameControls.Default.Move.performed += OnMovePerformed;
			_gameControls.Enable();
		}

		private void OnDisable()
		{
			_gameControls.Disable();
			_gameControls.Default.Move.performed -= OnMovePerformed;
		}

		private void OnMovePerformed(InputAction.CallbackContext context)
		{
			_actorBase.SetMove(context.ReadValue<Vector2>());
		}
	}
}
