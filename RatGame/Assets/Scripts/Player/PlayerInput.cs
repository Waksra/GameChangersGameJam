using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using World;

namespace Player
{
	public class PlayerInput : MonoBehaviour
	{
		[SerializeField] private float moveTargetLead = 5.0f;
		[SerializeField] private float moveCommandInterval = 0.25f;
		[Space(10)] 
		[SerializeField] private bool debugPositions = false;
		
		private Vector2 _moveInput;

		private Vector3 _swarmCenter = Vector3.zero;
		private Vector3 _targetPosition = Vector3.zero;
		
		private GameControls _gameControls;

		private void Awake()
		{
			_gameControls = new GameControls();
		}

		private void OnEnable()
		{
			_gameControls.Default.Move.performed += OnMovePerformed;
			_gameControls.Default.Restart.performed += OnRestartPerformed;
			_gameControls.Enable();
			
			StartCoroutine(MoveCommandCoroutine());
		}

		private void OnDisable()
		{
			_gameControls.Disable();
			_gameControls.Default.Move.performed -= OnMovePerformed;
			
			StopAllCoroutines();
		}

		private IEnumerator MoveCommandCoroutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(moveCommandInterval);
				
				Vector3 inputInWorld = new Vector3(_moveInput.x, 0, _moveInput.y);
				
				_swarmCenter = BoidManager.Instance.GetCenterOfSwarm();
				_targetPosition = _swarmCenter + inputInWorld * moveTargetLead;
				GridController.Instance.PathToWorldPosition(_targetPosition);
			}
		}

		private void OnRestartPerformed(InputAction.CallbackContext obj)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void OnMovePerformed(InputAction.CallbackContext context)
		{
			_moveInput = context.ReadValue<Vector2>();
		}

		private void OnDrawGizmos()
		{
			if (!debugPositions)
				return;
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(_swarmCenter, 2.0f);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(_targetPosition, 2f);
		}
	}
}
