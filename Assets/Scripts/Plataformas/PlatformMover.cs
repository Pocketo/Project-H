using UnityEngine;

/// <summary>
/// Componente que hace que el jugador (con CharacterController) 
/// se mueva junto con la plataforma.
/// Añádelo a cualquier plataforma móvil.
/// </summary>
public class PlatformMover : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool debugMode = false;
    
    private Vector3 lastPosition;
    private Transform playerOnPlatform;
    
    private void Start()
    {
        lastPosition = transform.position;
    }
    
    private void FixedUpdate()
    {
        if (playerOnPlatform != null)
        {
            // Calcular el movimiento de la plataforma
            Vector3 platformMovement = transform.position - lastPosition;
            
            // Mover al jugador la misma cantidad
            CharacterController cc = playerOnPlatform.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.Move(platformMovement);
            }
            
            if (debugMode)
            {
                Debug.Log($"Platform moved: {platformMovement}, Player: {playerOnPlatform.name}");
            }
        }
        
        lastPosition = transform.position;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            // Verificar que el jugador está encima (no de lado)
            if (IsPlayerOnTop(collision))
            {
                playerOnPlatform = collision.transform;
                
                if (debugMode)
                    Debug.Log($"Player {collision.gameObject.name} entered platform");
            }
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == playerOnPlatform)
        {
            playerOnPlatform = null;
            
            if (debugMode)
                Debug.Log($"Player {collision.gameObject.name} left platform");
        }
    }
    
    private bool IsPlayer(GameObject obj)
    {
        return ((1 << obj.layer) & playerLayer) != 0;
    }
    
    private bool IsPlayerOnTop(Collision collision)
    {
        // Verificar que la mayoría de los puntos de contacto están arriba
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y < 0.5f) // Normal apuntando hacia arriba
            {
                return false;
            }
        }
        return true;
    }
}
