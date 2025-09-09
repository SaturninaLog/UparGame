using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;             // Jugador
    public Vector3 offset = new Vector3(0, 5, -10); // Posici�n relativa a jugador
    public float smoothSpeed = 5f;       // Suavidad del seguimiento
    public bool lookAtPlayer = true;     // Si quieres que la c�mara mire al jugador

    void LateUpdate()
    {
        if (!target) return;

        // Calcula posici�n deseada
        Vector3 desiredPosition = target.position + offset;

        // Movimiento suave
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Opcional: que mire al jugador
        if (lookAtPlayer)
        {
            transform.LookAt(target.position);
        }
    }
}
