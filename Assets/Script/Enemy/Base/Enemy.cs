using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    [Header("References")]
    public NavMeshAgent agent;
    public Transform[] patrolPoints;
    public EnemyStateMachine stateMachine;

    public int currentPoint;

    [Header("Vision")]
    public float visionAngle = 110;
    public int visionRange = 7;
    public float lostDuration = 2f;
    public float targetHeight = 1f;
    public LayerMask visionMask;
    public Transform target;
    private Transform currentTarget;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        stateMachine = GetComponent<EnemyStateMachine>();

        stateMachine.ChangeState(EnemyStateMachine.EnemyState.Patrol);

        //agent.SetDestination(patrolPoints[currentPoint].position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }



    public void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }

    public void Chase(Transform target)
    {
        agent.SetDestination(target.position);
    }

    public bool CanSeePlayer()
    {
        if (target == null)
            return false;

        Vector3 origin = transform.position;

        Vector3 targetPosition = target.transform.position;

        Vector3 direction = targetPosition - origin;
        float angle = Vector3.Angle(direction, transform.forward);

        Debug.DrawRay(origin, direction.normalized * visionRange, Color.red);

        float currentDis = Vector3.Distance(target.transform.position, transform.position);

        RaycastHit hit;

        Debug.DrawRay(
    origin,
    transform.forward * visionRange,
    Color.green,
    0.1f);
        if (angle <= visionAngle && currentDis <= visionRange)
        {
            if (Physics.Linecast(origin, targetPosition, out hit))
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }


    }

   

    public void FindTargets()
    {
        /*
         targets = GameObject.FindGameObjectsWithTag("Player");

         foreach (GameObject target in targets)
         {
             Debug.Log("Verificando: " + target.name);

             bool canSee = CanSeePlayer(target.transform);

             Debug.Log("Pode enxergar? " + canSee);

             if (CanSeePlayer(target.transform))
             {
                 currentTarget = target.transform;
                 stateMachine.ChangeState(EnemyStateMachine.EnemyState.Chase);

                 Debug.Log("Player detected: " + target.name);
                 return;
             }
         }

         currentTarget = null;
        */
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        float halfAngle = visionAngle * 0.5f;

        Vector3 centerDirection = transform.forward * visionRange;

        Vector3 leftDirection =
            Quaternion.AngleAxis(-halfAngle, Vector3.up) *
            transform.forward *
            visionRange;

        Vector3 rightDirection =
            Quaternion.AngleAxis(halfAngle, Vector3.up) *
            transform.forward *
            visionRange;

        Gizmos.color = Color.green;

        Gizmos.DrawRay(origin, centerDirection);
        Gizmos.DrawRay(origin, leftDirection);
        Gizmos.DrawRay(origin, rightDirection);

        Gizmos.DrawLine(
            origin + leftDirection,
            origin + rightDirection
        );
    }

}
