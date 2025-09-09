using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target; // La c�mara o el jugador que quieres seguir

    void LateUpdate()
    {
        if (target == null)
        {
            // Si no est� asignado, busca autom�ticamente la c�mara principal
            if (Camera.main != null)
                target = Camera.main.transform;
            else
                return;
        }

        // Hace que el objeto mire hacia el target (jugador/c�mara)
        transform.LookAt(transform.position + target.forward);
    }
}
