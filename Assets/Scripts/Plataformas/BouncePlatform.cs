using UnityEngine;

public class BouncePlatform : PlatformBase
{
    [Header("Impulso")]
    [SerializeField] private float bounceForce = 15f;
    [SerializeField] private bool overrideVerticalVelocity = true;
    [SerializeField] private LayerMask targetLayers;
    
    [Header("Animación")]
    [SerializeField] private float squashAmount = 0.2f;
    [SerializeField] private float squashDuration = 0.15f;
    
    private Vector3 originalScale;
    private bool isSquashing = false;
    
    protected override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;
        
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            // Verificar que el objeto está cayendo sobre la plataforma
            if (collision.relativeVelocity.y < 0)
            {
                ApplyBounce(collision.gameObject);
            }
        }
    }
    
    private void ApplyBounce(GameObject target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        CharacterController cc = target.GetComponent<CharacterController>();
        
        if (rb != null)
        {
            if (overrideVerticalVelocity)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = 0;
                rb.linearVelocity = vel;
            }
            
            rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
        }
        else if (cc != null)
        {
            // Para CharacterController, necesitas modificar tu PlayerController
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplyExternalForce(Vector3.up * bounceForce);
            }
        }
        
        if (!isSquashing)
        {
            StartCoroutine(SquashAnimation());
        }
    }
    
    private System.Collections.IEnumerator SquashAnimation()
    {
        isSquashing = true;
        
        Vector3 squashedScale = originalScale;
        squashedScale.y *= (1f - squashAmount);
        squashedScale.x *= (1f + squashAmount * 0.5f);
        squashedScale.z *= (1f + squashAmount * 0.5f);
        
        float elapsed = 0f;
        while (elapsed < squashDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, elapsed / squashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < squashDuration)
        {
            transform.localScale = Vector3.Lerp(squashedScale, originalScale, elapsed / squashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = originalScale;
        isSquashing = false;
    }
}
