using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SelecNivel : MonoBehaviour
{
    public static SelecNivel Instance;
    public RectTransform Jugador;
    private Nivel selec;

    void Awake()
    {
        Instance = this;
    }

    public void Selec(Nivel punto)
    {
        selec = punto;
        Jugador.position = punto.transform.position;
    }

    public void Escena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    private void Update()
    {
        if (selec != null && Input.GetKeyDown(KeyCode.Return))
        {
            Escena(selec.nombreEscena);
        }
    }
}
