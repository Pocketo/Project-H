using UnityEngine;

public class CurarArbol : MonoBehaviour
{
    [Header("El prefab nuevo que reemplazará a este")]
    public GameObject newPrefab;

    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
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
        if (newPrefab == null)
        {
            Debug.LogWarning("No se asignó un newPrefab al script.");
            return;
        }

        // Guardamos la posición y rotación actuales
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        // Destruir este objeto
        Destroy(gameObject);

        // Instanciar el nuevo prefab
        Instantiate(newPrefab, pos, rot);
    }
}