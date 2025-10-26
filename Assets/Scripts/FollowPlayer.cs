using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    private Transform player;
    [Header("Dependencies")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    //Congelación
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    //Patrullaje
    [Header("Patrolling")]
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;
    //Ataque
    [Header("Attack Settings")]
    public float timeBetweenAttacks = 2f;
    public bool useRange = true;
    public bool useMelee = true;
    public int meleeDamage = 10;
    public float meleeRange = 2f;
    private bool alreadyAttacked;
    //Detección
    [Header("Detection & States")]
    public float sightRange = 15f;
    public float attackRange = 3f;
    private bool playerInSight, playerInAttack;
    //Animaciones
    private Animator ani;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("EnemyAI: No se encontró un objeto con el tag 'Player'.");
    }
    private void Update()
    {
        if (player == null) return;
        // Si está congelado, solo esperar
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0) UnfreezeEnemy();
            return;
        }
        // Detección del jugador
        playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttack = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (playerInAttack)
            AttackPlayer();
        else if (playerInSight)
            ChasePlayer();
        else
            Patrol();
    }
    //Patrulla
    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.sqrMagnitude < 1f) walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, walkPointRange, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            if (Physics.Raycast(walkPoint, Vector3.down, 5f, whatIsGround))
                walkPointSet = true;
        }
    }
    //Persecución
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    //Ataque
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if (!alreadyAttacked)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (useMelee && distance <= meleeRange)
            {
                DoMeleeDamage();
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void DoMeleeDamage()
    {
        if (player.TryGetComponent<PlayerHealth>(out PlayerHealth ph)) ;
    }

    private void ResetAttack() => alreadyAttacked = false;

    // --- Vida y Congelación ---
    public void TakeDamage(float damage)
    {
        if (isFrozen) return; // mientras está congelado no recibe daño
    }
    public void FreezeEnemy(float duration = 2f)
    {
        if (!isFrozen)
        {
            isFrozen = true;
            freezeTimer = duration;
            agent.isStopped = true;
            Debug.Log($"{gameObject.name} fue congelado por {duration} segundos.");
        }
    }
    private void UnfreezeEnemy()
    {
        isFrozen = false;
        agent.isStopped = false;
        Debug.Log($"{gameObject.name} ya no está congelado.");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
