using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class Nivel : MonoBehaviour, IPointerClickHandler
{
    public string nombreEscena;
    public Action<Nivel> Seleccion;
    private float Click;
    private float DClick = 0.3f;

    public void OnPointerClick(PointerEventData eventData)
    {
        float timeClick = Time.time - Click;
        if (timeClick <= DClick)
        {
            if (Seleccion != null)
            {
                SelecNivel.Instance.Escena(nombreEscena);
            }
            
        }
        else
        {
            Seleccion?.Invoke(this);
        }
        Click = Time.time;
    }
}
