using UnityEngine;

public class AirTarget : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Destroy the Zero if health is depleted
        }
    }
}
