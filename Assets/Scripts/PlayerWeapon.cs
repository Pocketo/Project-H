using System;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private bool frozen = false;
    [SerializeField] AudioClip sound;
    private AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null && sound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log("Vida restante: " + enemyHealth.GetCurrentHealth());
                // Reproducir sonido
                if (sound != null && audioSource != null)
                {
                    AudioSource.PlayClipAtPoint(sound, transform.position);
                }
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