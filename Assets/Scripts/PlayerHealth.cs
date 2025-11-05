using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vidas")]
    [SerializeField] private int totalLives = 3;
    private int currentLives;
    private bool isDead = false;
    [SerializeField] private float deathAnimationDuration = 3f;
    [SerializeField] private int maxHealthPerLife=100;
    private int currentHealth;
    [Header("Invulnerabilidad")]
    [SerializeField] private float invulnerabilityTime = 2f;
    private bool invulnerable=false;
    [SerializeField] InventoryUIController inventoryUIController;
    [SerializeField] GameObject UIDeath;
    private Animator ani;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        Time.timeScale = 1f;
        if (UIDeath == null)
        {
            UIDeath.SetActive(false);
        }
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
        if (currentLives > 0)
        {
            currentLives--;
            ani.SetTrigger("Hit");
            inventoryUIController.RestaCorazones(currentLives);
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
        
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.RoundToInt(amount);
        while (currentHealth > maxHealthPerLife && currentLives < totalLives)
        {
            currentHealth -= maxHealthPerLife;
            currentLives++;
            Debug.Log("Vida extra, vida total: " + currentHealth);
            if (inventoryUIController != null)
            {
                inventoryUIController.RecuperaCorazones(currentLives-1);
            }
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

    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Desactivar controles
        GetComponent<PlayerController>().enabled = false;
        
        // Animación
        if (ani != null)
            ani.SetTrigger("Death");
        
        // Mostrar UI después
        StartCoroutine(ShowDeathUIAfterDelay());
    }
    
    private IEnumerator ShowDeathUIAfterDelay()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        
        if (UIDeath != null)
            UIDeath.SetActive(true);
        
        Time.timeScale = 0;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Application.Quit();
    }
    
}
