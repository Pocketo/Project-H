using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverUI;
    public TextMeshProUGUI gameOverText;
    public Button reiniciarGame;
    public Button menu;
    private bool gameOver = false;
    public GameObject Inventario;
    public string mainMenu = "MainMenu";
    
    public CanvasGroup fade;
    public CanvasGroup titulo;
    public CanvasGroup subtitulo;
    public CanvasGroup botones;
    public Camera camara;
    public float fadeTime = 0.6f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }


        if (reiniciarGame != null)
        {
            reiniciarGame.onClick.AddListener(Reiniciar);
        }


        if (menu != null)
        {
            menu.onClick.AddListener(CargarMenu);
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            if (botones != null && !botones.interactable) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reiniciar();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CargarMenu();
            }
        }
    }

    public void ShowGameOver()
    {
        if (gameOver) return;
        gameOver = true;

        if (Inventario != null)
        {
            Inventario.SetActive(false);
        }
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if(fade!=null) 
        if (titulo != null)
        {
            titulo.alpha = 0;
            titulo.transform.localScale = Vector3.one;
        }
        if (subtitulo != null) subtitulo.alpha = 0;
        if (botones != null)
        {
            botones.alpha = 0;
            botones.interactable = true;
        }
        Time.timeScale = 0f;
        StartCoroutine(Animacion());
    }

    IEnumerator Animacion()
    {
        if(fade != null)
        yield return StartCoroutine(FadeCanvasGroup(fade,fade.alpha,0.6f, fadeTime*1.5f,true));

        if (camara != null)
        {
            float t = 0;
            float zoom = 1.2f;
            float inicio=camara.fieldOfView;
            float fin = inicio + 5f;
            while (t < zoom)
            {
                float eased =Mathf.SmoothStep(0,1,t/zoom);
                camara.fieldOfView = Mathf.Lerp(inicio,fin,t/zoom);
                t+=Time.unscaledDeltaTime;
                yield return null;
            }
        }

        if (titulo != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(titulo, 0, 1, 0.3f, true));
            yield return StartCoroutine(ScalePop(titulo.transform, 0.8f, 1f, 0.8f));
        }

        yield return new WaitForSecondsRealtime(0.5f);
        if (subtitulo != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(subtitulo,0,1,0.8f,true));
        }

        if (botones != null)
        {
            botones.alpha = 0;
            RectTransform rt = botones.GetComponent<RectTransform>();
            Vector2 originalPos = rt.anchoredPosition;
            Vector2 startPos = originalPos + new Vector2(0, -80f);
            rt.anchoredPosition = startPos;
            yield return StartCoroutine(SlideAndFade(botones,rt,startPos,originalPos,0.6f));

        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, bool unscaled)
    {
        float t = 0;
        while (t < duration)
        {
            float eased =Mathf.SmoothStep(0,1,t/duration);
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            t+=unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

    IEnumerator ScalePop(Transform target, float startScale, float endScale, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            float eased= Mathf.Sin((t/duration)*Mathf.PI*0.5f);
            float scale = Mathf.Lerp(startScale, endScale, eased);
            target.localScale = Vector3.one * scale;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        target.localScale = Vector3.one * endScale;
    }

    IEnumerator SlideAndFade(CanvasGroup group, RectTransform rt, Vector2 startPos, Vector2 endPos, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            float eased= Mathf.SmoothStep(0,1,t/duration); 
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            group.alpha = Mathf.Lerp(0, 1, eased);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.anchoredPosition = endPos;
        group.alpha = 1;
    }

   
    public void Reiniciar()
    {
        StartCoroutine(SalidaFade(SceneManager.GetActiveScene().name));
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CargarMenu()
    {
        StartCoroutine(SalidaFade(mainMenu));
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenu);
    }

    IEnumerator SalidaFade(string nombreEscena)
    {
        if (botones != null)
        {
            botones.interactable = false;
        }

        if (fade != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(fade,fade.alpha,1f, 0.5f,true));
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}


