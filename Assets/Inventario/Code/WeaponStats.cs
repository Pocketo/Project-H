using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon Stats")]
public class WeaponStats : ScriptableObject 
{
    [Header("Arma")]
    public GameObject weaponPrefab;
    [Header("Estadisticas")]
    public string weaponName = "Nueva arma";
    public bool freeze = false;
    public Vector3 visualScale = Vector3.one;
    public Sprite weaponIcon;
}
