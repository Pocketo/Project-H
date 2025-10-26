using UnityEngine;
using System.Collections;

public class FallingPlatform : PlatformBase
{
    [Header("Configuración de Caída")]
    [SerializeField] private float shakeTime = 0.5f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float fallDelay = 0.3f;
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float respawnTime = 3f;
    
    [Header("Detección de Jugador")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector3 detectionSize = new Vector3(1f, 0.5f, 1f);
    [SerializeField] private Vector3 detectionOffset = Vector3.up * 0.5f;
    
    private bool isShaking = false;
    private bool isFalling = false;
    private bool hasPlayerOnTop = false;
    private Rigidbody rb;
    private Collider platformCollider;
    
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        platformCollider = GetComponent<Collider>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    
    private void FixedUpdate()
    {
        if (!isActive) return;
        
        CheckPlayerOnTop();
        
        if (hasPlayerOnTop && !isShaking && !isFalling)
        {
            StartCoroutine(FallSequence());
        }
    }
    
    private void CheckPlayerOnTop()
    {
        Vector3 checkPosition = transform.position + detectionOffset;
        hasPlayerOnTop = Physics.CheckBox(checkPosition, detectionSize / 2f, transform.rotation, playerLayer);
    }
    
    private IEnumerator FallSequence()
    {
        isShaking = true;
        
        // Fase de temblor
        float elapsed = 0f;
        while (elapsed < shakeTime)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            randomOffset.y = Mathf.Abs(randomOffset.y); // Solo temblar hacia arriba/lados
            transform.position = startPosition + randomOffset;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = startPosition;
        
        // Esperar antes de caer
        yield return new WaitForSeconds(fallDelay);
        
        // Caer
        isFalling = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.down * fallSpeed;
        
        // Desactivar colisión para que no empuje al jugador
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }
        
        // Esperar y reaparecer
        if (respawnTime > 0f)
        {
            yield return new WaitForSeconds(respawnTime);
            RespawnPlatform();
        }
    }
    
    private void RespawnPlatform()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }
        
        isShaking = false;
        isFalling = false;
        hasPlayerOnTop = false;
    }
    
    protected override void OnReset()
    {
        StopAllCoroutines();
        RespawnPlatform();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = isFalling ? Color.red : (isShaking ? Color.yellow : Color.green);
        Vector3 checkPosition = transform.position + detectionOffset;
        Gizmos.DrawWireCube(checkPosition, detectionSize);
    }
}
