using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon Stats")]
public class WeaponStats : ScriptableObject 
{
    public enum WeaponType {Heavy, Fast}
    [Header("Arma")]
    public GameObject weaponPrefab;
    [Header("Estadisticas")]
    public string weaponName = "Nueva arma";
    public WeaponType type = WeaponType.Fast;
    public int damage = 10;
    public float attackSpeed = 1f; // Usado como Cooldown
    public bool freeze = false;
    public Vector3 visualScale = Vector3.one;
    public Sprite weaponIcon;

}
