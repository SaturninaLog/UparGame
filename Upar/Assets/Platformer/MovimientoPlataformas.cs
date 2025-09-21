using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movimiento")]
    public Vector3 direccion = Vector3.right; // Dirección de movimiento (X, Y o Z)
    public float distancia = 3f;              // Qué tan lejos se mueve
    public float velocidad = 2f;              // Velocidad de movimiento

    private Vector3 puntoInicial;
    private Vector3 puntoDestino;
    private bool yendoHaciaDestino = true;

    void Start()
    {
        puntoInicial = transform.position;
        puntoDestino = puntoInicial + direccion.normalized * distancia;
    }

    void Update()
    {
        // Mueve hacia destino o regresa
        if (yendoHaciaDestino)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoDestino, velocidad * Time.deltaTime);

            if (Vector3.Distance(transform.position, puntoDestino) < 0.1f)
                yendoHaciaDestino = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoInicial, velocidad * Time.deltaTime);

            if (Vector3.Distance(transform.position, puntoInicial) < 0.1f)
                yendoHaciaDestino = true;
        }
    }
}
