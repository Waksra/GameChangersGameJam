using System.Collections;
using Actor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Peasant : MonoBehaviour
{
	private enum PeasantState
	{
		WANDERING,
		RUNNING,
		IDLE,
		BEING_EATEN
	}

	private PeasantState State;

	private ActorBase ActorBase = null;
	private Animator Animator = null;

	private Vector3 RunningDirection = Vector3.zero;
	private Vector3 WalkDestination = Vector3.zero;
	private Vector3 MoveDirection = Vector3.zero;

	private Coroutine CurrentWanderingRoutine = null;

	[SerializeField] private LayerMask RatLayer;

	[SerializeField] private float RatCheckDistance = 1.0f;
	[SerializeField] private float SenseDelay = 0.2f;
	[SerializeField] private float RunningSpeed = 5.0f;
	[SerializeField] private float WalkingSpeed = 2.0f;
	[SerializeField] private Vector2 WalkRadiusMinMax = new Vector2(4, 10);
	[SerializeField] private float IdleDelay = 5.0f;
	
	private static readonly int IsRunning = Animator.StringToHash("IsRunning");
	private static readonly int IsWalking = Animator.StringToHash("IsWalking");

	private void Awake()
	{
		ActorBase = GetComponent<ActorBase>();
		Animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (State == PeasantState.RUNNING)
		{

			Vector2 moveDir = new Vector2(RunningDirection.x, RunningDirection.z);
			ActorBase.SetMove(moveDir);
			
			transform.forward = RunningDirection;
		}
	}

	private void Start()
	{
		State = PeasantState.IDLE;
		StartCoroutine(SenseRats());
		
		Invoke("StartWalking", IdleDelay);
	}

	private void StartRunning()
	{
		State = PeasantState.RUNNING;
		ActorBase.SetMaxMoveSpeed(RunningSpeed);
		Animator.SetBool(IsRunning, true);
		Animator.SetBool(IsWalking, false);
	}

	private void StopRunning()
	{
		State = PeasantState.IDLE;
		ActorBase.SetMove(Vector2.zero);
		Animator.SetBool(IsRunning, false);
	}

	private void StartWalking()
	{
		State = PeasantState.WANDERING;
		ActorBase.SetMaxMoveSpeed(WalkingSpeed);
		Animator.SetBool(IsWalking, true);
		CurrentWanderingRoutine = StartCoroutine(Wander());
	}

	private IEnumerator SenseRats()
	{
		while (true)
		{
			
			Collider[] hits = Physics.OverlapSphere(transform.position, RatCheckDistance, RatLayer);
			if (hits.Length > 0)
			{
				Vector3 averageRatDirection = Vector3.zero;
				foreach (Collider c in hits)
				{
					averageRatDirection += transform.position - c.transform.position;
				}

				RunningDirection = averageRatDirection.normalized;
				RunningDirection.y = 0;
				StartRunning();
				if (State == PeasantState.WANDERING)
				{
					Animator.SetBool(IsWalking, false);
					StopCoroutine(CurrentWanderingRoutine);
				}
			}
			else
			{
				StopRunning();
			}
			yield return new WaitForSeconds(SenseDelay);
		}
	}

	private IEnumerator Wander()
	{
		Vector3 randomOnCircle = Random.onUnitSphere;
		randomOnCircle.y = 0;

		WalkDestination = transform.position + (randomOnCircle * Random.Range(WalkRadiusMinMax.x, WalkRadiusMinMax.y));

		while (Vector3.Distance(transform.position, WalkDestination) > 0.2f)
		{
			MoveDirection = (WalkDestination - transform.position);
		}
		
		yield return null;
	}
}
