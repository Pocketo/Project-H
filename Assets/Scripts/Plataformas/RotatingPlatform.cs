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
    
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Transform playerOnPlatform;
    
    protected override void Start()
    {
        base.Start();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
    
    private void FixedUpdate()
    {
        if (!isActive) return;
        
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
    
    private void OnCollisionEnter(Collision collision)
    {
        if (movePlayerWithPlatform && IsPlayer(collision.gameObject) && IsPlayerOnTop(collision))
        {
            playerOnPlatform = collision.transform;
        }
    }
    
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