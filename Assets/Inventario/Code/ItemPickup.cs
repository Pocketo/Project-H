using UnityEngine;
using WeaponData=WeaponStats;
public class ItemPickup : MonoBehaviour
{
    [Header("Item")] 
    public ConsumableData itemData;
    public WeaponData weaponData;

    private InventoryManager playerInventory;

    private bool playerIsInRange = false;
    
    //deteccion de proximidad

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
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
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
    }

    private string GetItemName()
    {
        if (weaponData != null) return weaponData.weaponName;
        if(itemData!=null) return itemData.itemName;
        return "Objeto desconocido";
    }
}
