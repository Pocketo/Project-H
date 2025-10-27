using UnityEngine;

public class RotatingPlatform : PlatformBase
{
    [Header("Rotación")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private bool rotateInLocalSpace = true;
    
    [Header("Detección de Jugador")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool movePlayerWithPlatform = true;
    [SerializeField] private Vector3 detectionSize = new Vector3(5f, 0.5f, 5f);
    [SerializeField] private Vector3 detectionOffset = Vector3.up * 0.5f;
    
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Transform playerOnPlatform;
    private bool hasPlayerOnTop;
    
    protected override void Start()
    {
        base.Start();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
    
    private void FixedUpdate()
    {
        if (!isActive) return;
        
        // Detectar jugador en cada frame
        if (movePlayerWithPlatform)
        {
            CheckPlayerOnTop();
        }
        
        float angle = rotationSpeed * Time.fixedDeltaTime;
        
        if (rotateInLocalSpace)
        {
            transform.Rotate(rotationAxis, angle, Space.Self);
        }
        else
        {
            transform.Rotate(rotationAxis, angle, Space.World);
        }
        
        // Mover al jugador con la plataforma (incluyendo rotación)
        if (movePlayerWithPlatform && playerOnPlatform != null)
        {
            // Movimiento de posición
            Vector3 platformMovement = transform.position - lastPosition;
            
            // Movimiento por rotación
            Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(lastRotation);
            Vector3 offset = playerOnPlatform.position - transform.position;
            Vector3 rotatedOffset = rotationDelta * offset;
            Vector3 rotationMovement = rotatedOffset - offset;
            
            CharacterController cc = playerOnPlatform.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.Move(platformMovement + rotationMovement);
            }
        }
        
        lastPosition = transform.position;
        lastRotation = transform.rotation;
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
    
    private void OnDrawGizmos()
    {
        // Dibujar área de detección del jugador
        Gizmos.color = hasPlayerOnTop ? Color.green : Color.yellow;
        Vector3 checkPosition = transform.position + detectionOffset;
        Gizmos.DrawWireCube(checkPosition, detectionSize);
    }
}