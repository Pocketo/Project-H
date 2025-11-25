using UnityEngine;

public class CurarArbol : MonoBehaviour
{
    [Header("El prefab nuevo que reemplazará a este")]
    public GameObject newPrefab;

    [SerializeField] private GameObject ui;

    private bool playerInside = false;
    
    private AudioSource audioSource;
    [SerializeField] private AudioClip efecto;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            ui.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            ui.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.C))
        {
            ReplacePrefab();
        }
    }

    private void ReplacePrefab()
    {
        
        // Crear un objeto temporal para reproducir el sonido
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = efecto;
        tempSource.Play();
        Destroy(tempAudio, efecto.length);

// Ocultar UI
        ui.SetActive(false);

// Guardar pos y rot antes
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

// Instanciar el nuevo árbol
        Instantiate(newPrefab, pos, rot);

// Destruir el árbol viejo al final
        Destroy(gameObject);

    }
}