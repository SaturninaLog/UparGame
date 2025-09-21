using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Guardamos la posición inicial como primer checkpoint
        CheckpointManager.instance.SetStartPosition(transform.position);
    }

    public void Respawn()
    {
        // Apenas muere, pantalla negra aparece y luego respawnea
        DeathScreenManager.instance.PlayDeathFade(() =>
        {
            Vector3 respawnPos = CheckpointManager.instance.GetRespawnPosition();

            controller.enabled = false;
            transform.position = respawnPos;
            controller.enabled = true;

            Debug.Log("Jugador respawneado en: " + respawnPos);
        });
    }



}
