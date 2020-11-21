using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoidManager : StaticManager<BoidManager>
{
	private List<Boid> Boids = new List<Boid>();
	private GameControls GameControls;
	private int CurrentID = 0;

	public Vector2 CurrentInput = Vector2.zero;
	
	private void Awake()
	{
		GameControls = new GameControls();
		GameControls.Enable();

		CurrentID = 0;
		GameControls.Default.Move.performed += OnMovePerformed;
	}

	private void OnMovePerformed(InputAction.CallbackContext obj)
	{
		CurrentInput = obj.ReadValue<Vector2>();
	}

	public List<Boid> GetBoidsInDistance(float Distance, Boid Caller)
	{
		List<Boid> boidsInDistance = new List<Boid>();

		Vector3 callerPosition = Caller.transform.position;
		
		foreach (Boid b in Boids)
		{
			if (b.ID == Caller.ID)
				continue;
			
			if(Vector3.Distance(b.transform.position, callerPosition) < Distance)
			{
				boidsInDistance.Add(b);
			}
		}
		return boidsInDistance;
	}

	public int AddBoid(Boid BoidToAdd)
	{
		Boids.Add(BoidToAdd);
		CurrentID++;
		return CurrentID;
	}
}
