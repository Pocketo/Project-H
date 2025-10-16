using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;
    
    private int maxHealth;
    private Animator ani;
    void Start()
    {
        maxHealth = health;
        ani = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        ani.SetTrigger("Hit");
        health -= damage;
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Vida: " + health);
        
        if (health <= 0)
        {
            ani.SetTrigger("Death");
            Destroy(gameObject, 3.0f);
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
