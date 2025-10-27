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
    [SerializeField] private Vector3 detectionSize = new Vector3(1f, 0.5f, 1f);
    [SerializeField] private Vector3 detectionOffset = Vector3.up * 0.5f;
    
    private int currentWaypointIndex = 0;
    private float journeyLength;
    private float startTime;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private int direction = 1; // 1 = forward, -1 = backward
    
    // Variables para mover al jugador
    private Vector3 lastPosition;
    private Transform playerOnPlatform;
    private bool hasPlayerOnTop;
    
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
        
        // Detectar jugador en cada frame
        if (movePlayerWithPlatform)
        {
            CheckPlayerOnTop();
        }
        
        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                MoveToNextWaypoint();
            }
            
            // Mover al jugador incluso mientras espera
            if (playerOnPlatform != null)
            {
                Vector3 platformMovement = transform.position - lastPosition;
                CharacterController cc = playerOnPlatform.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.Move(platformMovement);
                }
            }
            
            lastPosition = transform.position;
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
    
    private void CheckPlayerOnTop()
    {
        Vector3 checkPosition = transform.position + detectionOffset;
        Collider[] hits = Physics.OverlapBox(checkPosition, detectionSize / 2f, transform.rotation, playerLayer);
        
        if (hits.Length > 0)
        {
            // Encontró al jugador
            if (playerOnPlatform == null)
            {
                playerOnPlatform = hits[0].transform;
            }
            hasPlayerOnTop = true;
        }
        else
        {
            // No hay jugador
            playerOnPlatform = null;
            hasPlayerOnTop = false;
        }
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
        
        // Dibujar área de detección del jugador
        Gizmos.color = hasPlayerOnTop ? Color.green : Color.yellow;
        Vector3 checkPosition = transform.position + detectionOffset;
        Gizmos.DrawWireCube(checkPosition, detectionSize);
    }
}