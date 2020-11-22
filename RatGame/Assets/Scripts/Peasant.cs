using System.Collections;
using Actor;
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

	private PeasantState State = PeasantState.IDLE;

	private ActorBase ActorBase = null;
	private Animator Animator = null;
	private PreySetter PreySetter = null;

	private Vector3 RunningDirection = Vector3.zero;
	private Vector3 WalkDestination = Vector3.zero;
	private Vector3 MoveDirection = Vector3.zero;

	private IEnumerator CurrentWanderingRoutine = null;

	private Rigidbody Body = null;
	private Joint Joint = null;
	
	[SerializeField] private LayerMask RatLayer;

	[SerializeField] private float RatCheckDistance = 1.0f;
	[SerializeField] private float SenseDelay = 0.2f;
	[SerializeField] private float RunningSpeed = 5.0f;
	[SerializeField] private float WalkingSpeed = 2.0f;
	[SerializeField] private Vector2 WalkRadiusMinMax = new Vector2(4, 10);
	[SerializeField] private float IdleDelay = 5.0f;
	[SerializeField] private float DamageDelay = 0.2f;
	[SerializeField] private float DamagePerDelay = 5.0f;
	
	private static readonly int TimeToWalk = Animator.StringToHash("TimeToWalk");
	private static readonly int NoRatNearby = Animator.StringToHash("NoRatNearby");
	private static readonly int RatFound = Animator.StringToHash("RatFound");
	private static readonly int WalkingDestinationReached = Animator.StringToHash("WalkingDestinationReached");

	private void Awake()
	{
		ActorBase = GetComponent<ActorBase>();
		Animator = GetComponent<Animator>();

		Body = GetComponent<Rigidbody>();

		PreySetter = GetComponent<PreySetter>();

		CurrentWanderingRoutine = Wander();
	}

	private void Update()
	{
		if (State == PeasantState.RUNNING)
		{
			Vector2 moveDir = new Vector2(RunningDirection.x, RunningDirection.z) * RunningSpeed;
			ActorBase.SetMove(moveDir);
			
			transform.forward = RunningDirection;
		}
		else if (State == PeasantState.WANDERING)
		{
			Vector2 moveDir = new Vector2(MoveDirection.x, MoveDirection.z) * WalkingSpeed;
			ActorBase.SetMove(moveDir);

			transform.forward = MoveDirection;
		}
	}

	private void OnDisable()
	{
		Destroy(Joint);
		Body.velocity = Vector3.zero;
		CurrentWanderingRoutine = Wander();
		StartIdling();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (State == PeasantState.BEING_EATEN)
			return;
		
		if (RatLayer == (RatLayer | (1 << other.gameObject.layer)))
		{
			StartBeingEaten(other.rigidbody);
		}
	}

	private void Start()
	{
		StartIdling();
		StartCoroutine(SenseRats());
	}

	private void StartIdling()
	{
		if (State == PeasantState.BEING_EATEN)
			return;
		State = PeasantState.IDLE;
		StopCoroutine(CurrentWanderingRoutine);
		CurrentWanderingRoutine = Wander();
		StartCoroutine(CurrentWanderingRoutine);
	}

	private void StartRunning()
	{
		if (State == PeasantState.BEING_EATEN)
			return;
		State = PeasantState.RUNNING;
		ActorBase.SetMaxMoveSpeed(RunningSpeed);
		Animator.SetTrigger(RatFound);
	}

	private void StopRunning()
	{
		if (State == PeasantState.BEING_EATEN)
			return;
		StartIdling();
		ActorBase.SetMove(Vector2.zero);
		Animator.SetTrigger(NoRatNearby);
	}

	private void StartWalking()
	{
		if (State == PeasantState.BEING_EATEN)
			return;
		State = PeasantState.WANDERING;
		ActorBase.SetMaxMoveSpeed(WalkingSpeed);
		Animator.SetTrigger(TimeToWalk);
	}

	private void StopWalking()
	{
		if (State == PeasantState.BEING_EATEN)
			return;
		StartIdling();
		ActorBase.SetMove(Vector2.zero);
		Animator.SetTrigger(WalkingDestinationReached);
	}

	private void StartBeingEaten(Rigidbody rat)
	{
		State = PeasantState.BEING_EATEN;
		Body.constraints = RigidbodyConstraints.None;
		Joint = gameObject.AddComponent<HingeJoint>();
		Joint.connectedBody = rat;
		Body.useGravity = true;
		// Animator.enabled = false;
		PreySetter.RemoveAsPrey();
		StopAllCoroutines();
		StartCoroutine(TakeDamage());
	}

	private IEnumerator TakeDamage()
	{
		while (true)
		{
			ActorBase.Damage(DamagePerDelay);
			Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f, RatLayer);
			foreach (var h in hits)
			{
				h.GetComponent<ActorBase>().Heal(DamagePerDelay);
			}
			yield return new WaitForSeconds(DamageDelay);
		}
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
					StopCoroutine(CurrentWanderingRoutine);
				}
			}
			else if(State == PeasantState.RUNNING)
			{
				StopRunning();
			}
			yield return new WaitForSeconds(SenseDelay);
		}
	}

	private IEnumerator Wander()
	{
		yield return new WaitForSeconds(IdleDelay);
		StartWalking();
		
		Vector3 randomOnCircle = Random.onUnitSphere;
		randomOnCircle.y = 0;

		WalkDestination = transform.position + (randomOnCircle * Random.Range(WalkRadiusMinMax.x, WalkRadiusMinMax.y));

		WalkDestination.y = transform.position.y;
		while (Vector3.Distance(transform.position, WalkDestination) > 0.2f)
		{
			MoveDirection = (WalkDestination - transform.position);
			yield return null;
		}

		StopWalking();
		yield return null;
	}
}
