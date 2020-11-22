using UnityEngine;

namespace Actor
{
	public class ActorBase : MonoBehaviour
	{
		private MovementController _movementController;
		private bool _hasMovementController;

		private void Awake()
		{
			_hasMovementController = TryGetComponent(out _movementController);
		}

		public void SetMove(Vector2 movement)
		{
			if(!_hasMovementController) return;

			_movementController.MoveVector = movement;
		}

		public void SetMaxMoveSpeed(float maxMovementSpeed)
		{
			if (!_hasMovementController) return;
			
			_movementController.maxSpeed = maxMovementSpeed;
		}
		
	}
}
