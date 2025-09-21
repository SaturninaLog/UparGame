using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target; // La cámara o el jugador que quieres seguir

    void LateUpdate()
    {
        if (target == null)
        {
            // Si no está asignado, busca automáticamente la cámara principal
            if (Camera.main != null)
                target = Camera.main.transform;
            else
                return;
        }

        // Hace que el objeto mire hacia el target (jugador/cámara)
        transform.LookAt(transform.position + target.forward);
    }
}
