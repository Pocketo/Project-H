using UnityEngine;
using WeaponData= WeaponStats;
public class PlayerAttack : MonoBehaviour
{
    [Header("Dependencias")]
    public InventoryManager inventoryManager;
    
    private float nextAttackTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("PlayerAttack: Falta asignar referencia a inventario");
        }
    }
    private WeaponData CurrentWeapon
    {
        get
        {
            if(inventoryManager==null) return null;
            return inventoryManager.GetEquippedWeapon();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInventoryInput();
        WeaponData currentWeapon = CurrentWeapon;
        if (currentWeapon == null) return;

        if (Time.time < nextAttackTime) return;
    }

    private void HandleInventoryInput()
    {
        if (inventoryManager == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventoryManager.EquipWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventoryManager.EquipWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventoryManager.EquipWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryManager.UnequipWeapon();
        }
    }
}
