using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoidManager : StaticManager<BoidManager>
{
	private List<Boid> Boids = new List<Boid>();

	private GameControls GameControls;

	public Vector2 CurrentInput = Vector2.zero;
	
	private void Awake()
	{
		GameControls = new GameControls();
		GameControls.Enable();

		GameControls.Default.Move.performed += OnMovePerformed;
	}

	private void OnMovePerformed(InputAction.CallbackContext obj)
	{
		CurrentInput = obj.ReadValue<Vector2>();
	}

	public List<Boid> GetBoidsInDistance(float Distance, Vector3 CallerPosition)
	{
		List<Boid> boidsInDistance = new List<Boid>();
		foreach (Boid g in Boids)
		{
			if(Vector3.Distance(g.transform.position, CallerPosition) < Distance)
			{
				boidsInDistance.Add(g);
			}
		}
		return boidsInDistance;
	}

	public void AddBoid(Boid BoidToAdd)
	{
		Boids.Add(BoidToAdd);
	}
}
