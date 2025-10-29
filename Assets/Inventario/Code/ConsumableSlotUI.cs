using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConsumableSlotUI : MonoBehaviour
{
    public Image slotIcon;
    public TextMeshProUGUI countText;

    public void UpdateSlot(ConsumableData itemData, int count)
    {
        if (itemData != null && count > 0)
        {
            slotIcon.sprite = itemData.itemIcon;
            slotIcon.enabled = true;
            countText.text = count.ToString();
            gameObject.SetActive(true);
        }
        else
        {
            slotIcon.enabled = false;
            countText.text = "";
            gameObject.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
