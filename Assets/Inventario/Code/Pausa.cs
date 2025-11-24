using UnityEngine;
using UnityEngine.SceneManagement; 
public class PauseMenu : MonoBehaviour
{
    public GameObject pausaMenuUI; 

    public static bool Pausar = false; 

    public string mainMenuSceneName = "MainMenu"; 

    void Start()
    {
        if (pausaMenuUI != null)
        {
            pausaMenuUI.SetActive(false);
        }
        Time.timeScale = 1f; 
        Pausar = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Pausar)
            {
                Continuar(); 
            }
            else
            {
                Pausa(); 
            }
        }
    }

    public void Continuar()
    {
        if (pausaMenuUI != null)
        {
            pausaMenuUI.SetActive(false); 
        }
        Time.timeScale = 1f; 
        Pausar = false; 
        Debug.Log("Juego reanudado.");
    }

    public void Pausa()
    {
        if (pausaMenuUI != null)
        {
            pausaMenuUI.SetActive(true); 
        }
        Time.timeScale = 0f; 
        Pausar = true; 
        Debug.Log("Juego pausado.");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Menu()
    {
        Time.timeScale = 1f; 
        Pausar = false;
        Debug.Log("Cargando menú principal...");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}