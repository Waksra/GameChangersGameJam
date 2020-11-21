using System;
using System.Collections.Generic;
using Actor;
using UnityEngine;

[SelectionBase]
public class Boid : MonoBehaviour
{
    [SerializeField] private float SeparationRadius = 1.0f;
    [SerializeField] private float AlignmentRadius = 1.0f;
    [SerializeField] private float CohesionRadius = 1.0f;

    [SerializeField] private float SeparationWeight = 1.0f;
    [SerializeField] private float AlignmentWeight = 1.0f;
    [SerializeField] private float CohesionWeight = 1.0f;
    [SerializeField] private float InputWeight = 1.0f;

    [SerializeField] private float MaxSteering = 1.0f;
    [System.NonSerialized]
    public Vector3 Velocity;

    [System.NonSerialized]
    public int ID;
    private ActorBase ActorBase = null;
    
    private void Awake()
    {
        ActorBase = GetComponent<ActorBase>();
    }

    void Start()
    {
        ID = BoidManager.Instance.AddBoid(this);
    }

    void Update()
    {
        Vector3 acceleration = CalculateDesiredDirection() * Time.deltaTime;
        Velocity += acceleration;

        float speed = Velocity.magnitude;
        Vector3 dir = Velocity / speed;

        Velocity = speed * dir;
        dir.y = 0;
        
        transform.forward = dir;
        ActorBase.SetMove(new Vector2(Velocity.x, Velocity.z));
    }

    private Vector3 CalculateDesiredDirection()
    {
        return (CalculateSeparation() + CalculateAlignment() + CalculateCohesion() + CalculateInput());
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero;

        List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(SeparationRadius, this);
        if (neighbours.Count <= 0)
            return Vector3.zero;
        
        foreach (Boid b in neighbours)
        {
            separation += transform.position - b.transform.position;
        }

        separation = Vector3.ClampMagnitude(separation, MaxSteering);
        
        return separation * SeparationWeight;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 alignment = Vector3.zero;

        List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(AlignmentRadius, this);
        if (neighbours.Count <= 0)
            return Vector3.zero;
        
        foreach (Boid b in neighbours)
        {
            alignment += b.Velocity;
        }

        alignment = Vector3.ClampMagnitude(alignment, MaxSteering);
        
        return alignment * AlignmentWeight;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 cohesion = Vector3.zero;
        
        List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(CohesionRadius, this);
        if (neighbours.Count <= 0)
            return Vector3.zero;
        
        foreach (Boid b in neighbours)
        {
            cohesion += b.transform.position;
        }

        cohesion /= neighbours.Count;
        
        cohesion -= transform.position;

        cohesion = Vector3.ClampMagnitude(cohesion, MaxSteering);
        
        return cohesion * CohesionWeight;
    }

    private Vector3 CalculateInput()
    {
        Vector2 inputDirection = BoidManager.Instance.CurrentInput;
        return new Vector3(inputDirection.x, 0, inputDirection.y) * InputWeight;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AlignmentRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, SeparationRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, CohesionRadius);
    }

    
}
