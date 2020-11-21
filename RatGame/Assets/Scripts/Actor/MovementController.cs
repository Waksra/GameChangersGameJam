using UnityEngine;

namespace Actor
{
	[RequireComponent(typeof(Rigidbody))]
	public class MovementController : MonoBehaviour
	{
		[SerializeField, Range(0f, 1000f)] 
		public float maxSpeed = 10f;
		[SerializeField, Range(0f, 1000f)] 
		public float maxAcceleration = 10f;

		private Vector3 _desiredVelocity;

		private Rigidbody _body;

		public Vector2 MoveVector
		{
			set
			{
				value *= maxSpeed;
				_desiredVelocity.x = value.x;
				_desiredVelocity.z = value.y;
			}
		}

		private void Awake()
		{
			_body = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			Vector3 velocity = _body.velocity;
			_desiredVelocity.y = velocity.y;
			float maxVelocityChange = maxAcceleration;
        
			velocity = Vector3.ClampMagnitude(Vector3.MoveTowards(velocity, _desiredVelocity, maxVelocityChange), maxSpeed);
        
			_body.velocity = velocity;
		}
	}
}
