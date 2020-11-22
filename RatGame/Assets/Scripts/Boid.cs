using System.Collections;
using System.Collections.Generic;
using Actor;
using UnityEngine;
using UnityEngine.Analytics;
using World;

[SelectionBase]
public class Boid : MonoBehaviour
{
    [SerializeField] private float SeparationRadius = 1.0f;
    [SerializeField] private float AlignmentRadius = 1.0f;
    [SerializeField] private float CohesionRadius = 1.0f;
    [SerializeField] private float PreySeekRadius = 1.0f;
    [Space(10)]
    [SerializeField] private float SeparationWeight = 1.0f;
    [SerializeField] private float AlignmentWeight = 1.0f;
    [SerializeField] private float CohesionWeight = 1.0f;
    [SerializeField] private float FlowFieldWeight = 1.0f;
    [SerializeField] private float PreyWeight = 1.0f;
    [Space(10)]
    [SerializeField] private float MaxSteering = 1.0f;
    [SerializeField] private Vector2 MinMaxSpeed = new Vector2(0.1f, 3);
    [Space(10)]
    [SerializeField] private Vector2 MinMaxDelay = new Vector2(0, 1f);
    [SerializeField] private float SeperationFrequency = 0.1f;
    [SerializeField] private float AlignmentFrequency = 0.12f;
    [SerializeField] private float CohesionFrequency = 0.14f;
    [SerializeField] private float FlowFieldFrequency = 0.16f;
    [SerializeField] private float SeekPreyFrequency = 0.5f;
    [SerializeField] private float PreyDirectonFrequency = 0.18f;
    [Space(10)]
    [Range(1f, 3f)][SerializeField] private float PreyReselectDistanceFactor = 1.5f;


    [System.NonSerialized]
    public Vector3 Velocity = Vector3.zero;

    [System.NonSerialized]
    public int ID;
    private ActorBase ActorBase = null;

    private Transform OwnTransform;

    private List<ActorBase> _preyInRange;

    private ActorBase _targetPrey;
    private bool _hasTargetPrey = false;

    private Vector3 _separationAcceleration = Vector3.zero;
    private Vector3 _alignmentAcceleration = Vector3.zero;
    private Vector3 _cohesionAcceleration = Vector3.zero;
    private Vector3 _flowFieldAcceleration = Vector3.zero;
    private Vector3 _preyAcceleration = Vector3.zero;

    private void Awake()
    {
        ActorBase = GetComponent<ActorBase>();
        OwnTransform = transform;
    }

    void Start()
    {
        ID = BoidManager.Instance.AddBoid(this);
        StartCoroutine(StartAccelerationCalculationWithDelay());
    }

    void Update()
    {
        Vector3 acceleration = CalculateDesiredDirection() * Time.deltaTime;
        Velocity += acceleration;

        float speed = Velocity.magnitude;
        Vector3 dir = Vector3.zero; 
        if(speed != 0)
            dir = Velocity / speed;
        speed = Mathf.Clamp(speed, MinMaxSpeed.x, MinMaxSpeed.y);

        Velocity = speed * dir;
        dir.y = 0;
        
        OwnTransform.forward = dir;
        ActorBase.SetMove(new Vector2(Velocity.x, Velocity.z));
        Debug.DrawLine(OwnTransform.position, OwnTransform.position + Velocity, Color.blue);
    }

    private Vector3 CalculateDesiredDirection()
    {
        return _separationAcceleration + _alignmentAcceleration + _cohesionAcceleration + _flowFieldAcceleration 
               + _preyAcceleration;
    }

    private IEnumerator StartAccelerationCalculationWithDelay()
    {
        yield return new WaitForSeconds(Random.Range(MinMaxDelay.x, MinMaxDelay.y));
        StartCoroutine(CalculateSeparation());
        StartCoroutine(CalculateAlignment());
        StartCoroutine(CalculateCohesion());
        StartCoroutine(CalculateFlowFieldAcceleration());
        StartCoroutine(SeekPrey());
        StartCoroutine(CalculatePreyAcceleration());
    }

    private IEnumerator CalculateSeparation()
    {
        WaitForSeconds delay = new WaitForSeconds(SeperationFrequency);
        while (true)
        {
            yield return delay;
            
            _separationAcceleration = Vector3.zero;

            List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(SeparationRadius, this);
            if (neighbours.Count <= 0)
                continue;
        
            foreach (Boid b in neighbours)
            {
                _separationAcceleration += OwnTransform.position - b.transform.position;
            }

            _separationAcceleration = Vector3.ClampMagnitude(_separationAcceleration, MaxSteering);
        
            _separationAcceleration *= SeparationWeight;
        }
    }

    private IEnumerator CalculateAlignment()
    {
        WaitForSeconds delay = new WaitForSeconds(AlignmentFrequency);
        while (true)
        {
            yield return delay;
            
            _alignmentAcceleration = Vector3.zero; 

            List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(AlignmentRadius, this);
            if (neighbours.Count <= 0)
                continue;
        
            foreach (Boid b in neighbours)
            {
                _alignmentAcceleration += b.Velocity;
            }

            _alignmentAcceleration = Vector3.ClampMagnitude(_alignmentAcceleration, MaxSteering);
        
            _alignmentAcceleration *= AlignmentWeight;
        }
    }

    private IEnumerator CalculateCohesion()
    {
        WaitForSeconds delay = new WaitForSeconds(CohesionFrequency);
        while (true)
        {
            yield return delay;
            
            _cohesionAcceleration = Vector3.zero;
        
            List<Boid> neighbours = BoidManager.Instance.GetBoidsInDistance(CohesionRadius, this);
            if (neighbours.Count <= 0)
                continue;
        
            foreach (Boid b in neighbours)
            {
                _cohesionAcceleration += b.transform.position;
            }

            _cohesionAcceleration /= neighbours.Count;
        
            _cohesionAcceleration -= OwnTransform.position;

            _cohesionAcceleration = Vector3.ClampMagnitude(_cohesionAcceleration, MaxSteering);
        
            _cohesionAcceleration *= CohesionWeight;
        }
    }

    public IEnumerator CalculateFlowFieldAcceleration()
    {
        WaitForSeconds delay = new WaitForSeconds(FlowFieldFrequency);
        while (true)
        {
            yield return delay;
            
            _flowFieldAcceleration =
                GridController.Instance.GetDirectionFromWorldPosition(transform.position) * FlowFieldWeight;
        }
    }

    private IEnumerator SeekPrey()
    {
        WaitForSeconds delay = new WaitForSeconds(SeekPreyFrequency);
        while (true)
        {
            yield return delay;
            
            if(_hasTargetPrey)
                continue;
            
            _preyInRange = BoidManager.Instance.GetPreyInRadiusFrom(OwnTransform.position, PreySeekRadius);
            if(_preyInRange == null)
                continue;

            float minDistance = float.MaxValue;
            foreach (ActorBase prey in _preyInRange)
            {
                float distance = Vector3.Distance(OwnTransform.position, prey.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _targetPrey = prey;
                    _hasTargetPrey = true;
                }
            }
        }
    }

    private IEnumerator CalculatePreyAcceleration()
    {
        WaitForSeconds delay = new WaitForSeconds(PreyDirectonFrequency);
        while (true)
        {
            yield return delay;
            if(!_hasTargetPrey)
                continue;

            Vector3 preyPosition = _targetPrey.transform.position;
            preyPosition.y = OwnTransform.position.y;
            _preyAcceleration = preyPosition - OwnTransform.position;
            float distanceToPrey = _preyAcceleration.magnitude;
            float increasedRadius = PreySeekRadius * PreyReselectDistanceFactor;
            if (distanceToPrey > increasedRadius)
            {
                _hasTargetPrey = false;
                _preyAcceleration = Vector3.zero;
                continue;
            }
            _preyAcceleration /= PreySeekRadius;
            _preyAcceleration *= Mathf.Abs(increasedRadius - distanceToPrey) * PreyWeight;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, AlignmentRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position, SeparationRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, CohesionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, PreySeekRadius);
    }

    
}
