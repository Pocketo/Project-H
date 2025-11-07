using UnityEngine;
using System.Collections.Generic;
using WeaponData = WeaponStats;
using System.Linq;
public class InventoryManager : MonoBehaviour
{
    [Header("Arma Actual")] 
    public WeaponData equippedWeapon;

    public int equippedWeaponIndex = -1;
    [Header("Inventario")] 
    public List<WeaponData> weaponInventory = new List<WeaponData>();
    [Header("UI")]
    public InventoryUIController uiController;

    [Header("Arma")] public
        Transform weaponRespawn;

    private GameObject currentweaponInstance;
    
    [Header("Consumibles")]
    public Dictionary<ConsumableData, int> consumableInventory = new Dictionary<ConsumableData, int>();
    
    [Header("Referencias")]
    public PlayerHealth playerHealth;
    void Awake()
    {
        if (equippedWeapon != null)
        {
            if (!weaponInventory.Contains(equippedWeapon))
            {
                weaponInventory.Insert(0,equippedWeapon);
            }

            equippedWeaponIndex = weaponInventory.IndexOf(equippedWeapon);
            EquipVisuals(equippedWeapon);
            Debug.Log($"Arma Equipada: {equippedWeapon.weaponName}");
            if (uiController != null)
            {
                uiController.BuildWeaponSlots();
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager: No asigno arma");
        }
        //Aqui va la espada inicial de madera
        //equippedWeapon(WeaponD.Espada);
        if (playerHealth == null)
        {
            playerHealth= FindObjectOfType<PlayerHealth>();
        }
        
    }

    /// <summary>
    /// Devuelve el arma que el jugador tiene equipada
    /// </summary>
    /// <returns>El WeaponD.Weapon o null</returns>

    public WeaponData GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon == null) return;
        if (!weaponInventory.Contains(newWeapon))
        {
            weaponInventory.Add(newWeapon);
        }
        
        if (newWeapon == equippedWeapon) return;
        equippedWeaponIndex = weaponInventory.IndexOf(newWeapon);
        equippedWeapon = newWeapon;

        EquipVisuals(newWeapon);
        Debug.Log($"Arma Equipada: {equippedWeapon.weaponName}");

        if (uiController != null)
        {
            uiController.UpdateEquippedVisuals();
        }
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponInventory.Count)
        {
            Debug.LogWarning("InventoryManager: No asigno arma");
            return;
        }

        WeaponData newWeapon = weaponInventory[index];
        EquipWeapon(newWeapon);
    }

    public void UnequipWeapon()
    {
        if(equippedWeapon==null) return;
        equippedWeapon = null;
        equippedWeaponIndex = -1;
        EquipVisuals(null);
        Debug.Log("Arma guardada");
        if (uiController != null)
        {
            uiController.UpdateEquippedVisuals();
        }
    }

    public void AddWeapon(WeaponData weaponToAdd)
    {
        bool wasAdded=false;

        if (weaponToAdd != null)
        {
            if (!weaponInventory.Contains(weaponToAdd))
            {
                weaponInventory.Add(weaponToAdd);
                Debug.Log($"Se agrego {weaponToAdd.weaponName}");
                wasAdded = true;
            }
            
        }

        if (!weaponInventory.Contains(weaponToAdd))
        {
            weaponInventory.Add(weaponToAdd);
           
        }

        if (wasAdded && uiController != null)
        {
            uiController.BuildWeaponSlots();
        }
        if (equippedWeapon == null)
        {
            EquipWeapon(weaponInventory.Count -1);
        }
    }

    public void AddConsumable(ConsumableData itemToAdd)
    {
        if (itemToAdd == null)
        {
            Debug.LogWarning("InventoryManager: No asigno Consumible");
            return;
        }

        if (consumableInventory.ContainsKey(itemToAdd))
        {
            consumableInventory[itemToAdd]++;
        }
        else
        {
            consumableInventory.Add(itemToAdd, 1);
        }
        Debug.Log($"Agarraste{itemToAdd.itemName}. Cantidad total: {consumableInventory[itemToAdd]}");

        if (uiController != null)
        {
            uiController.UpdateConsumableUI();
        }
    }
    private void EquipVisuals(WeaponData newWeapon)
    {
        if (currentweaponInstance != null)
        {
            Destroy(currentweaponInstance);
            currentweaponInstance = null;
        }

        if (newWeapon == null || newWeapon.weaponPrefab == null || weaponRespawn == null)
        {
            return;
        }

        currentweaponInstance = Instantiate(
            newWeapon.weaponPrefab,
            weaponRespawn);
        currentweaponInstance.transform.localScale = newWeapon.visualScale;
        currentweaponInstance.transform.localPosition = Vector3.zero;
        currentweaponInstance.transform.localRotation = Quaternion.identity;

    }

    public void UseConsumable(ConsumableData item)
    {
        if (item == null||playerHealth==null||!consumableInventory.ContainsKey(item)||consumableInventory[item]<=0)
        {
            Debug.LogWarning("InventoryManager: No se puede sanar");
            return;
        }
        
        playerHealth.Heal(item.healthRestored);
        Debug.Log($"Usaste {item.itemName} curaste {item.healthRestored}");
        //animacion de curacion
        //GetComponent<Animator>().SetTrigger("Curacion);
        
        /*if (consumableInventory.Contains(item))
        {
            playerHealth.Heal(item.healthRestored);
            Debug.Log($"Usaste {item.itemName}. Curaste {item.healthRestored} ");
            
            //animacion
            GetComponent<Animator>().SetTrigger("item.animationTrigger");
        */
            if (item.destroyOnUse)
            {
                consumableInventory[item]--;
                if(consumableInventory[item]<=0)
                {
                    
                consumableInventory.Remove(item);
                Debug.Log($"{item.itemName} consumido");

                //uiController.UpdateConsumableUI();
            }
        }
        else
        {
            {
                Debug.Log("No tiene curación");
            }
        }

        if (uiController != null)
        {
            uiController.UpdateConsumableUI();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseFirstConsumable();
        }
    }

    public void UseFirstConsumable()
    {
        var firstConsumableEntry=consumableInventory.FirstOrDefault();
        if (firstConsumableEntry.Key == null || firstConsumableEntry.Value <= 0)
        {
            Debug.LogWarning("UseFirstConsumable: No tienes consumibles");
            return;
        }
        ConsumableData itemToUse = firstConsumableEntry.Key;
        UseConsumable(itemToUse);
    }
    
}
