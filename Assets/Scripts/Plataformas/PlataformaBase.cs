using UnityEngine;

/// <summary>
/// Clase base para todas las plataformas.
/// Hereda de esta para crear nuevos tipos.
/// </summary>
public abstract class PlatformBase : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] protected bool activeOnStart = true;
    [SerializeField] protected float activationDelay = 0f;
    
    protected Vector3 startPosition;
    protected Quaternion startRotation;
    protected bool isActive = false;
    
    protected virtual void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    
    protected virtual void Start()
    {
        if (activeOnStart)
        {
            if (activationDelay > 0f)
                Invoke(nameof(Activate), activationDelay);
            else
                Activate();
        }
    }
    
    public virtual void Activate()
    {
        isActive = true;
        OnActivate();
    }
    
    public virtual void Deactivate()
    {
        isActive = false;
        OnDeactivate();
    }
    
    public virtual void ResetPlatform()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        OnReset();
    }
    
    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }
    protected virtual void OnReset() { }
}