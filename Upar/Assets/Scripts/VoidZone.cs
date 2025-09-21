using UnityEngine;

public class VoidZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // asegúrate de que el Player tenga el tag "Player"
        {
            AudioManager.instance.PlayFX(AudioManager.instance.caidaPlayerFX);
            Debug.Log("Jugador cayó en la Void Zone ⚠️");
            PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }
}
