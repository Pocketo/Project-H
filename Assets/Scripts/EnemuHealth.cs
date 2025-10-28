using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;
    
    private int maxHealth;
    private Animator ani;
    private EnemyAI enemy;
    
    void Start()
    {
        maxHealth = health;
        ani = GetComponent<Animator>();
        enemy = GetComponent<EnemyAI>();
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
            
            enemy.enabled = false;

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
