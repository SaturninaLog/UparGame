using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // El jugador debe tener tag "Player"
        {
            Debug.Log("💀 Jugador murió");
            other.GetComponent<PlayerController>().Die(); // Llamamos a la función Die()
        }
    }
}
