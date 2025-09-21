using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] objetos; // Aquí arrastras tus casas, arbustos, árboles

    [Header("Configuración")]
    public Transform jugador; // El jugador que avanza
    public float distanciaSpawn = 30f; // Cada cuántos metros generar
    public float rangoX = 5f; // Rango horizontal (izquierda/derecha)
    public float rangoY = 2f; // Rango de altura
    public int cantidadPorBloque = 3; // Cuántos objetos generar por bloque

    private float ultimaPosicionZ = 0f;

    void Update()
    {
        // Si el jugador avanza lo suficiente, genera nuevo bloque
        if (jugador.position.z > ultimaPosicionZ - distanciaSpawn)
        {
            GenerarBloque();
            ultimaPosicionZ += distanciaSpawn;
        }
    }

    void GenerarBloque()
    {
        for (int i = 0; i < cantidadPorBloque; i++)
        {
            GameObject prefab = objetos[Random.Range(0, objetos.Length)];

            Vector3 posicion = new Vector3(
                Random.Range(-rangoX, rangoX),
                Random.Range(0f, rangoY),
                ultimaPosicionZ + Random.Range(0f, distanciaSpawn)
            );

            Instantiate(prefab, posicion, Quaternion.identity);
        }
    }
}
