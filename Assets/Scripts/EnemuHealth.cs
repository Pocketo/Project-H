using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;
    
    private int maxHealth;
    private Animator ani;
    private EnemyAI enemy;
    private bool isDead = false; // Añadido para evitar múltiples llamadas
    
    void Start()
    {
        maxHealth = health;
        ani = GetComponent<Animator>();
        enemy = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage)
    {
        // Si ya está muerto, no procesar más daño
        if (isDead) return;
        
        ani.SetTrigger("Hit");
        health -= damage;
        
        // Asegurar que la vida no baje de 0
        health = Mathf.Max(health, 0);
        
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Vida: " + health);
        
        if (health <= 0)
        {
            isDead = true; // Marcar como muerto
            ani.SetTrigger("Death");
            enemy.enabled = false;
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