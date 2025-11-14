using UnityEngine;
using UnityEngine.UI;

public class Tecla : MonoBehaviour
{
    [Header("UI")]
    public GameObject messageUI;       // UI que mostrará el mensaje (un Panel, un Texto, etc.)
    public string messageText = "Presiona C para recoger";  
    public Text uiText;                // El componente Text dentro del panel

    private bool playerInside = false;

    private void Start()
    {
        if (messageUI != null)
            messageUI.SetActive(false);
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
        if (playerInside && Input.GetKeyDown(KeyCode.E)||Input.GetKeyDown(KeyCode.C))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        if (messageUI != null)
            messageUI.SetActive(false);

       
    }
}