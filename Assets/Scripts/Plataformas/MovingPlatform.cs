using UnityEngine;

public class MovingPlatform : PlatformBase
{
    [Header("Movimiento")]
    [SerializeField] private Vector3[] waypoints = new Vector3[2];
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool useLocalSpace = true;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float waitTimeAtPoint = 0.5f;
    
    [Header("Opciones")]
    [SerializeField] private bool loop = true;
    [SerializeField] private bool pingPong = true;
    
    [Header("Detección de Jugador")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool movePlayerWithPlatform = true;
    
    private int currentWaypointIndex = 0;
    private float journeyLength;
    private float startTime;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private int direction = 1; // 1 = forward, -1 = backward
    
    // Variables para mover al jugador
    private Vector3 lastPosition;
    private Transform playerOnPlatform;
    
    protected override void Start()
    {
        base.Start();
        
        // Convertir waypoints a posiciones del mundo si es necesario
        if (useLocalSpace)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = startPosition + waypoints[i];
            }
        }
        
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0];
            if (waypoints.Length > 1)
            {
                journeyLength = Vector3.Distance(waypoints[0], waypoints[1]);
                startTime = Time.time;
            }
        }
        
        lastPosition = transform.position;
    }
    
    private void FixedUpdate()
    {
        if (!isActive || waypoints.Length < 2) return;
        
        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                MoveToNextWaypoint();
            }
            return;
        }
        
        MovePlatform();
        
        // Mover al jugador con la plataforma
        if (movePlayerWithPlatform && playerOnPlatform != null)
        {
            Vector3 platformMovement = transform.position - lastPosition;
            CharacterController cc = playerOnPlatform.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.Move(platformMovement);
            }
        }
        
        lastPosition = transform.position;
    }
    
    private void MovePlatform()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;
        
        // Aplicar curva de animación
        float curvedFraction = movementCurve.Evaluate(fractionOfJourney);
        
        int nextIndex = currentWaypointIndex + direction;
        if (nextIndex >= 0 && nextIndex < waypoints.Length)
        {
            transform.position = Vector3.Lerp(waypoints[currentWaypointIndex], waypoints[nextIndex], curvedFraction);
        }
        
        // Llegó al destino
        if (fractionOfJourney >= 1f)
        {
            currentWaypointIndex = nextIndex;
            
            if (currentWaypointIndex >= waypoints.Length - 1 || currentWaypointIndex <= 0)
            {
                if (pingPong)
                {
                    direction *= -1;
                }
                else if (loop)
                {
                    currentWaypointIndex = 0;
                    transform.position = waypoints[0];
                }
                else
                {
                    isActive = false;
                    return;
                }
            }
            
            isWaiting = true;
            waitTimer = waitTimeAtPoint;
        }
    }
    
    private void MoveToNextWaypoint()
    {
        int nextIndex = currentWaypointIndex + direction;
        if (nextIndex >= 0 && nextIndex < waypoints.Length)
        {
            journeyLength = Vector3.Distance(waypoints[currentWaypointIndex], waypoints[nextIndex]);
            startTime = Time.time;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        
        Gizmos.color = Color.cyan;
        Vector3 basePos = Application.isPlaying ? Vector3.zero : (useLocalSpace ? transform.position : Vector3.zero);
        
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 worldPos = basePos + waypoints[i];
            Gizmos.DrawWireSphere(worldPos, 0.3f);
            
            if (i < waypoints.Length - 1)
            {
                Gizmos.DrawLine(worldPos, basePos + waypoints[i + 1]);
            }
            
            if (loop && i == waypoints.Length - 1)
            {
                Gizmos.DrawLine(worldPos, basePos + waypoints[0]);
            }
        }
    }
    
    // Detectar cuando el jugador sube a la plataforma
    private void OnCollisionEnter(Collision collision)
    {
        if (movePlayerWithPlatform && IsPlayer(collision.gameObject) && IsPlayerOnTop(collision))
        {
            playerOnPlatform = collision.transform;
        }
    }
    
    // Detectar cuando el jugador deja la plataforma
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == playerOnPlatform)
        {
            playerOnPlatform = null;
        }
    }
    
    private bool IsPlayer(GameObject obj)
    {
        return ((1 << obj.layer) & playerLayer) != 0;
    }
    
    private bool IsPlayerOnTop(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y < 0.5f)
                return false;
        }
        return true;
    }
}
