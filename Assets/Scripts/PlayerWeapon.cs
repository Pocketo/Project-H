using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private bool frozen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log("Vida restante: " + enemyHealth.GetCurrentHealth());
            }
        }
        else if (other.CompareTag("Death")&&frozen==true)
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            EnemyAI ia = other.gameObject.GetComponent<EnemyAI>();
            
            if (enemyHealth != null)
            {
                ia.agent.isStopped = true;
                enemyHealth.TakeDamage(damage);
                Debug.Log("Vida restante: " + enemyHealth.GetCurrentHealth());
            }
        }
    }
}