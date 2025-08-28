using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // el jugador
    public Vector3 offset;         // desplazamiento de la cámara
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (!target) return;

        // posición deseada (siempre detrás y arriba del jugador)
        Vector3 desiredPosition = target.position + offset;

        // suavizado (para que no se sienta brusco)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        // opcional: mantener mirando al frente
        transform.LookAt(target.position + Vector3.forward * 10f);
    }
}
