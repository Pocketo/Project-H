using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PickupItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject messageUI;
    [SerializeField] private string messageText = "Presiona E para recoger";  
    [SerializeField] private Text uiText;
    
    [Header("Configuración")]
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private float destroyDelay = 0f;
    
    [Header("Audio (Opcional)")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float volume = 1f;
    
    [Header("Eventos")]
    [SerializeField] private UnityEvent onPickup;
    
    private bool playerInside = false;
    private AudioSource audioSource;

    private void Start()
    {
        if (messageUI != null)
            messageUI.SetActive(false);
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (uiText != null)
                uiText.text = messageText;

            if (messageUI != null)
                messageUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (messageUI != null)
                messageUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        // Ocultar UI
        if (messageUI != null)
            messageUI.SetActive(false);
        
        // Reproducir sonido
        if (pickupSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }
        
        // Invocar evento
        onPickup?.Invoke();
        
        Debug.Log($"Recogido: {gameObject.name}");
        
        // Destruir objeto
        if (destroyOnPickup)
        {
            if (destroyDelay > 0f)
                Destroy(gameObject, destroyDelay);
            else
                Destroy(gameObject);
        }
    }
}