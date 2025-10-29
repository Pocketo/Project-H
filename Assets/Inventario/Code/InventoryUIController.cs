using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using WeaponData=WeaponStats;
public class InventoryUIController : MonoBehaviour
{
    [Header("Dependencias")]
    public InventoryManager inventoryManager;

    public GameObject slotPrefab;
    public Transform slotContainer;
    private List<GameObject> activeSlots = new List<GameObject>();
    [Header("Consumibles UI")]
    public GameObject consumableRoot;
    public ConsumableSlotUI consumableSlotPrefab;
    private ConsumableSlotUI activeConsumableSlot;
    [SerializeField] private List<GameObject> listaCorazones;
    [SerializeField] private Sprite corazonDes;
    [SerializeField] private Sprite corazonAct;

    void Awake()
    {
        BuildConsumableSlots();
        UpdateConsumableUI();
        BuildWeaponSlots();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("InventoryUIController::Start()");
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryUI necesita referencia");
            return;
        }
        BuildConsumableSlots();
        BuildWeaponSlots();
    }

    public void BuildWeaponSlots()
    {
        Debug.Log($"[DEBUG UI] Intentando construir {inventoryManager.weaponInventory.Count} slots.");

        if (slotPrefab == null)
        {
            Debug.LogError("[DEBUG UI] prefab nulo");
            return;
        }
        foreach (GameObject slot in activeSlots)
        {
            Destroy(slot);
        }
        activeSlots.Clear();

        for (int i = 0; i < inventoryManager.weaponInventory.Count; i++)
        {
            WeaponData weapon = inventoryManager.weaponInventory[i];
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);
            newSlot.name = $"WeaponSlot_{i + 1}";
            Image iconImage = newSlot.transform.Find("Icon")?.GetComponent<Image>();

            if (iconImage != null)
            {
                iconImage.sprite = weapon.weaponIcon;
                iconImage.color=Color.white;
            }
            else
            {
                Debug.LogError($"[UI ERROR] No hay componente Image o el objeto hijo 'WeaponIcon' en el prefab: {slotPrefab.name}");

            }
            
            TextMeshProUGUI slotNumberText= newSlot.transform.GetComponentInChildren<TextMeshProUGUI>();
            if (slotNumberText != null)
            {
                slotNumberText.text=(i+1).ToString();
            }
            else
            {
                {
                    UnityEngine.UI.Text legacyText= newSlot.GetComponentInChildren<UnityEngine.UI.Text>();
                    if (legacyText != null)
                    {
                        legacyText.text=(i+1).ToString();
                    }
                    else
                    {
                        {
                            Debug.LogWarning($"[UI ERROR] No hay un componente TextMeshProUGUI o Text en el prefab {slotPrefab.name}.");
                        }
                    }
                }
            }
            activeSlots.Add(newSlot);
        }

        UpdateEquippedVisuals();
    }

    public void BuildConsumableSlots()
    {
        if (consumableSlotPrefab == null)
        {
            Debug.LogError("[DEBUG UI] prefab nulo");
            return;
        }
        if (consumableRoot == null)
        {
            Debug.LogError("[DEBUG UI] Nulo");
            return;
        }
        if (activeConsumableSlot == null)
        {
            activeConsumableSlot = Instantiate(consumableSlotPrefab, consumableRoot.transform).GetComponent<ConsumableSlotUI>();
            if (activeConsumableSlot != null)
            {
                Debug.Log("[DEBUG UI] No se puede tener el script ConsumableSlotUI del prefab");
            }
        }
    }

    public void UpdateConsumableUI()
    {
        InventoryManager inventory= inventoryManager;
        if (inventory == null)
        {
            Debug.LogError("[DEBUG UI] Nulo");
            return;
        }
        if (activeConsumableSlot == null)
        {
            Debug.LogError("[UI ERROR] La ranura es nula");
            BuildConsumableSlots();
            if (activeConsumableSlot == null) return;
        }

        var firstConsumableEntry = inventory.consumableInventory.FirstOrDefault();

        if (firstConsumableEntry.Key != null && firstConsumableEntry.Value >0)
        {
            activeConsumableSlot.UpdateSlot(firstConsumableEntry.Key, firstConsumableEntry.Value);
        }
        else
        {
            activeConsumableSlot.UpdateSlot(null,0);
            activeConsumableSlot.gameObject.SetActive(false);
        }

        
    }

    public void UpdateEquippedVisuals()
    {
        if (inventoryManager == null) return;
        WeaponData currentEquipped = inventoryManager.GetEquippedWeapon();
        for (int i = 0; i < activeSlots.Count; i++)
        {
           GameObject slot = activeSlots[i];
           WeaponData weaponInSlot= inventoryManager.weaponInventory[i];

           Transform indicatorTransform = slot.transform.Find("Indicator");
           if (indicatorTransform != null)
           {
               bool isEquipped = (currentEquipped != null && currentEquipped == weaponInSlot);
               indicatorTransform.gameObject.SetActive(isEquipped);
           }
        }
    }

    public void RestaCorazones(int indice)
    {
        Image imagenCorazon= listaCorazones[indice].GetComponent<Image>();
        imagenCorazon.sprite = corazonDes;
        return;
    }

    public void RecuperaCorazones(int indice)
    {
        if (indice >= 0 && indice < listaCorazones.Count)
        {
            Image imagenCorazon= listaCorazones[indice].GetComponent<Image>();
            if (corazonAct != null)
            {
                imagenCorazon.sprite = corazonAct;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
