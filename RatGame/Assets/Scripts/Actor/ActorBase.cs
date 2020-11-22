using System;
using UnityEngine;

namespace Actor
{
	public class ActorBase : MonoBehaviour
	{
		private MovementController _movementController;
		private bool _hasMovementController;

		private Damageable _damageable;
		private bool _hasDamageable;

		private void Awake()
		{
			_hasMovementController = TryGetComponent(out _movementController);
			_hasDamageable = TryGetComponent(out _damageable);
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

		public void Damage(float amount)
		{
			if(!_hasDamageable) return;

			_damageable.Damage(amount);
		}

		public void Heal(float amount)
		{
			if(!_hasDamageable) return;

			_damageable.Heal(amount);
		}

		public void Kill()
		{
			if(!_hasDamageable) return;
			
			_damageable.Kill();
		}
	}
}
