using UnityEngine;

[CreateAssetMenu(fileName="New Consumable", menuName="Game Data/Consumable Data")]
public class ConsumableData : ScriptableObject
{
    [Header("Identificación")] 
    public string itemName = "Fresa curativa";
    public Sprite itemIcon;

    [Header("Efecto")] 
    public int healthRestored = 20;
    public string animationTrigger = "UseConsumable";

    [Header("Inventario")]
    public bool destroyOnUse = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
