using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;
    
    private int maxHealth;
    
    void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Vida: " + health);
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
