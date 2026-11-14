using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detección y Combate")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float attackRange = 2.2f;
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Patrulla")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float chaseSpeed = 4.5f;

    private NavMeshAgent agent;
    private Transform player;
    private HealthSystem playerHealth;
    private HealthSystem myHealth;
    private int currentPatrolIndex;
    private float nextAttackTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myHealth = GetComponent<HealthSystem>();

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthSystem>();
        }
    }

    private void Update()
    {
        if (myHealth != null && myHealth.IsDead)
        {
            if (agent.enabled) agent.enabled = false;
            gameObject.SetActive(false);
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (player == null || !agent.enabled || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            if (Time.time >= nextAttackTime && playerHealth != null && !playerHealth.IsDead)
            {
                nextAttackTime = Time.time + attackCooldown;
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"[EnemyAI] Enemigo atacó al jugador (-{attackDamage} HP)");
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        if (targetPoint == null) return;

        agent.SetDestination(targetPoint.position);

        if (!agent.pathPending && agent.remainingDistance <= 0.6f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
}