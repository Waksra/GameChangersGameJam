using System.Collections.Generic;
using Actor;
using UnityEngine;

public class BoidManager : StaticManager<BoidManager>
{
	private List<Boid> Boids = new List<Boid>();

	private List<ActorBase> Prey = new List<ActorBase>();

	private Vector3 _swarmDirection = Vector3.zero;
	private float _swarmDirectionAllowedAge = 0.1f;
	private float _swarmDirectionTimeStamp = 0.0f;

	public List<Boid> GetBoidsInDistance(float Distance, Boid Caller)
	{
		List<Boid> boidsInDistance = new List<Boid>();

		Vector3 callerPosition = Caller.transform.position;
		
		foreach (Boid b in Boids)
		{
			if (b == Caller)
				continue;
			
			if(Vector3.Distance(b.transform.position, callerPosition) < Distance)
			{
				boidsInDistance.Add(b);
			}
		}
		return boidsInDistance;
	}

	public Vector3 GetCenterOfSwarm()
	{
		Vector3 position = Vector3.zero;
		foreach (Boid boid in Boids)
		{
			position += boid.transform.position;
		}

		return position / Boids.Count;
	}

	public Vector3 GetAverageDirectionOfSwarm()
	{
		if (Time.time <= _swarmDirectionTimeStamp + _swarmDirectionAllowedAge)
			return _swarmDirection;
		
		Vector3 direction = Vector3.zero;
		foreach (Boid boid in Boids)
		{
			direction += boid.Velocity;
		}

		_swarmDirectionTimeStamp = Time.time;
		_swarmDirection = direction.normalized;
		return _swarmDirection;
	}

	public List<ActorBase> GetPreyInRadiusFrom(Vector3 position, float radius)
	{
		if (Prey.Count <= 0)
			return null;
		
		List<ActorBase> preyInRange = new List<ActorBase>();
		foreach (ActorBase prey in Prey)
		{
			if (Vector3.Distance(prey.transform.position, position) <= radius)
				preyInRange.Add(prey);
		}

		if(preyInRange.Count <= 0)
			return null;
		return preyInRange;
	}

	public void AddBoid(Boid BoidToAdd)
	{
		Boids.Add(BoidToAdd);
	}

	public void RemoveBoid(Boid boidToRemove)
	{
		if (Boids.Contains(boidToRemove))
			Boids.Remove(boidToRemove);
	}

	public void AddPrey(ActorBase preyToAdd)
	{
		Prey.Add(preyToAdd);
	}

	public void RemovePrey(ActorBase preyToRemove)
	{
		foreach (Boid b in Boids)
		{
			if (b.HasPrey(preyToRemove))
				b.RemovePrey();
		}
		Prey.Remove(preyToRemove);
	}
}
