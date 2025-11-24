using UnityEngine;
using UnityEngine.UI;
using WeaponData=WeaponStats;
public class ItemPickup : MonoBehaviour
{
    [Header("Item")] 
    public ConsumableData itemData;
    public WeaponData weaponData;

    private InventoryManager playerInventory;
    private Animator animator;

    private bool playerIsInRange = false;
    
    //deteccion de proximidad
    [Header("UI")]
    [SerializeField] private GameObject messageUI;
    [SerializeField] private string messageText = "Presiona E para recoger";  
    [SerializeField] private Text uiText;
    private bool playerInside = false;
    [Header("Audio (Opcional)")]
    [SerializeField] private AudioClip pickupSound;
    private AudioSource audioSource;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInventory= other.GetComponent<InventoryManager>();
            
            if (playerInventory != null)
            {
                playerIsInRange = true;
                string itemName = GetItemName();
                Debug.Log($"Presiona 'E' para agarrar {itemName}");
            }
            
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
            if (playerInventory != null)
            {
                playerIsInRange = false;
                playerInventory = null;
            }
            playerInside = false;

            if (messageUI != null)
                messageUI.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (messageUI != null)
            messageUI.SetActive(false);
        animator = FindObjectOfType<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
            animator.SetTrigger("Take");
        }
    }

    private void PickupItem()
    {
        if (playerInventory == null) return;
        bool itemWasPicked = false;
        if (weaponData != null)
        {
            //Agarrar y equipar el arma
            playerInventory.AddWeapon(weaponData);
            playerInventory.EquipWeapon(weaponData);
            Debug.Log($"Equipaste {weaponData.weaponName}");
            itemWasPicked = true;
        }
        else if (itemData != null)
        {
            playerInventory.AddConsumable(itemData);
            Debug.Log($"Agarraste{itemData.itemName}");
            itemWasPicked = true;
        }
        else
        {
            Debug.LogWarning("El objeto no tiene datos cosumibles");
        }

        if (itemWasPicked)
        {
            Destroy(gameObject);

        }   
        // Ocultar UI
        if (messageUI != null)
            messageUI.SetActive(false);
        
        // Reproducir sonido
        if (pickupSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }

    private string GetItemName()
    {
        if (weaponData != null) return weaponData.weaponName;
        if(itemData!=null) return itemData.itemName;
        return "Objeto desconocido";
    }
}
