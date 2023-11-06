using UnityEngine;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // Maximum health the player can have.
    private float currentHealth;

    [Header("Events")]
    public UnityEvent OnPlayerDeath; // Event triggered when the player's health drops to 0 or below.
    public UnityEvent OnHealthChanged; // Event triggered every time the health changes.

    public float CurrentHealth 
    {
        get { return currentHealth; }
        private set 
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged.Invoke();

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Start()
    {
        // Initialize the player's health to maximum at the start.
        CurrentHealth = maxHealth;
    }

    /// <summary>
    /// Decrease the player's health by a specified amount.
    /// </summary>
    /// <param name="amount">Amount of health to decrease.</param>
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
    }

    /// <summary>
    /// Increase the player's health by a specified amount.
    /// </summary>
    /// <param name="amount">Amount of health to increase.</param>
    public void Heal(float amount)
    {
        CurrentHealth += amount;
    }

    /// <summary>
    /// Handle player death.
    /// </summary>
    private void Die()
    {
        // Trigger the OnPlayerDeath event.
        OnPlayerDeath.Invoke();
        
        // Optional: Disable the player's movement or other components.
        // For example, you can add GetComponent<PlayerMovement>().enabled = false;
    }
}
