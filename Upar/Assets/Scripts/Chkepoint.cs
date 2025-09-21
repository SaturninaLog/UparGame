using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID = 0;
    public Animator checkpointAnimator; // referencia al Animator del objeto del checkpoint

    private bool activated = false; // para que solo se active una vez

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            AudioManager.instance.PlayFX(AudioManager.instance.CheckpointFX);
            activated = true;

            // ✅ Guardamos la posición en el manager
            CheckpointManager.instance.UpdateCheckpoint(checkpointID, transform.position);

            // ✅ Activamos animación
            if (checkpointAnimator != null)
            {
                checkpointAnimator.SetTrigger("Activate");
            }

            Debug.Log("Jugador activó checkpoint " + checkpointID);
        }
    }
}
