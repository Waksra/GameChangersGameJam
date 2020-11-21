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
    

    [SerializeField] private float MaxOutputVelocity = 3.0f;

    public Vector3 DesiredDirection;
    

    private ActorBase ActorBase = null;
    
    private void Awake()
    {
        ActorBase = GetComponent<ActorBase>();
    }

    void Start()
    {
        BoidManager.Instance.AddBoid(this);
    }

    void Update()
    {
        DesiredDirection = CalculateDesiredDirection() * Time.deltaTime;
        DesiredDirection.y = 0;
        DesiredDirection = Vector3.ClampMagnitude(DesiredDirection, MaxOutputVelocity);
        DesiredDirection /= MaxOutputVelocity;
        ActorBase.SetMove(new Vector2(DesiredDirection.x, DesiredDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(DesiredDirection), Time.deltaTime);
    }

    private Vector3 CalculateDesiredDirection()
    {
        return (CalculateSeparation() + CalculateAlignment() + CalculateCohesion() + CalculateInput());
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero;

        List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(SeparationRadius, transform.position);

        if (neighbours.Count <= 1)
            return separation;

        foreach (Boid b in neighbours)
        {
            separation += transform.position - b.transform.position;
        }
        
        return separation * SeparationWeight;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 alignment = Vector3.zero;

        List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(AlignmentRadius, transform.position);
        
        if (neighbours.Count <= 1)
            return alignment;

        foreach (Boid b in neighbours)
        {
            alignment += b.DesiredDirection;
        }
        
        alignment.Normalize();

        return alignment * AlignmentWeight;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 cohesion = Vector3.zero;
        
        List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(CohesionRadius, transform.position);

        if (neighbours.Count <= 1)
            return cohesion;
        
        foreach (Boid b in neighbours)
        {
            cohesion += b.transform.position;
        }

        cohesion /= neighbours.Count;

        cohesion -= transform.position;
        
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
