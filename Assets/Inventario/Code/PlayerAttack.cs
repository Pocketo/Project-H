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
        if (Input.GetMouseButtonDown(0))
        {
            Attack(currentWeapon);
            nextAttackTime = Time.time + currentWeapon.attackSpeed;
        }
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
    void Attack(WeaponData weaponToUse)
    {
        Ray ray=new Ray(transform.position, transform.forward);
        
        //Alcance de ataque 5 mts
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            if (hit.collider.TryGetComponent<EnemyAI>(out EnemyAI enemy))
            {
                Debug.Log($"Ataque con:  {weaponToUse.weaponName}");
                enemy.TakeDamage(weaponToUse.damage);

                if (weaponToUse.freeze)
                {
                    enemy.FreezeEnemy(2f);
                }
            }
        }
    }
}
