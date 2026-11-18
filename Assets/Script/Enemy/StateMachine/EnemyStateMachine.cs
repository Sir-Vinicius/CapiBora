using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{

    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase

    }

    [Header("State")]
    public EnemyState currentState;

    private Enemy enemy;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
       switch (currentState)
        {
            case EnemyState.Idle:
                
                break;
            case EnemyState.Patrol:
                
                enemy.Patrol();
                if(enemy.CanSeePlayer())
                {
                    ChangeState(EnemyState.Chase);
                }

                break;
            case EnemyState.Chase:
                
                enemy.Chase(enemy.target);
                if (!enemy.CanSeePlayer())
                {
                    ChangeState(EnemyState.Patrol);
                }

                break;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;


    }
}
