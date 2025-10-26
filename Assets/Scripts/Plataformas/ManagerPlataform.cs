using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    [Header("Gestión de Plataformas")]
    [SerializeField] private List<PlatformBase> platforms = new List<PlatformBase>();
    [SerializeField] private bool autoFindPlatforms = true;
    
    private void Start()
    {
        if (autoFindPlatforms)
        {
            platforms.AddRange(FindObjectsOfType<PlatformBase>());
        }
    }
    
    public void ActivateAll()
    {
        foreach (var platform in platforms)
        {
            platform.Activate();
        }
    }
    
    public void DeactivateAll()
    {
        foreach (var platform in platforms)
        {
            platform.Deactivate();
        }
    }
    
    public void ResetAll()
    {
        foreach (var platform in platforms)
        {
            platform.ResetPlatform();
        }
    }
    
    public void ActivatePlatform(int index)
    {
        if (index >= 0 && index < platforms.Count)
        {
            platforms[index].Activate();
        }
    }
}