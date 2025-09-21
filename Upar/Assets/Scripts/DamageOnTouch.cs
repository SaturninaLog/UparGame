using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damageAmount = 10; // Cantidad de daño

    private void OnCollisionEnter(Collision collision)
    {
        // Si el objeto que toca tiene el tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }

    // Si prefieres que también funcione con "Triggers" (colliders con IsTrigger activado)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }
}
