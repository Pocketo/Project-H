using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vidas")]
    public int totalLives = 3;
    public float currentLives;
    [Header("Salud")]
    public int maxHealthPerLife=100;
    public int currentHealth;
    [Header("Invulnerabilidad")]
    public float invulnerabilityTime = 2f;
    private bool invulnerable=false;

    private void Awake()
    {
        currentLives = totalLives;
        currentHealth = maxHealthPerLife;
        Debug.Log("Tiene: "+currentLives+" vidas");
        
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Recibio {damage} de daño. Vida: "+currentHealth);
        if (currentHealth <= 0)
        {
            LoseLife();
            //animacion muerte o algo asi
        }
        else
        {
            StartCoroutine(BecomeTemporarilyInvulnerable());
        }
    }

    private void LoseLife()
    {
        currentLives--;
        Debug.Log("Vidas restantes: " + currentLives);
        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            currentHealth = maxHealthPerLife;
            StartCoroutine(BecomeTemporarilyInvulnerable());
        }
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.RoundToInt(amount);
        while (currentHealth > maxHealthPerLife && currentLives < totalLives)
        {
            currentHealth -= maxHealthPerLife;
            currentLives++;
            Debug.Log("Vida extra, vida total: " + currentHealth);
        }
        currentHealth= Mathf.Clamp(currentHealth, 0, maxHealthPerLife);
        
        Debug.Log($"Jugador curado en : {amount} Vida:{currentHealth}");
    }
    private IEnumerator BecomeTemporarilyInvulnerable()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Player murio");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
}
