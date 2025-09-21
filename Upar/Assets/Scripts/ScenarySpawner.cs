using UnityEngine;

public class ScenerySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] treePrefabs;     // Árboles
    public GameObject[] bushPrefabs;     // Arbustos

    [Header("Player")]
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnZStart = 50f;      // Primer punto de spawn
    public float spawnDistance = 100f;   // Distancia en Z delante del jugador
    public float spawnInterval = 3f;     // 👈 Intervalo pequeño para llenar mejor
    public float xOffset = 20f;          // 👈 Distancia lateral (al borde de las casas)
    public int objectsPerSide = 2;       // 👈 Cuántos objetos por lado en cada fila
    public float sideSpacing = 2f;       // 👈 Espaciado entre objetos en el mismo lado

    private float lastSpawnZ;

    void Start()
    {
        lastSpawnZ = spawnZStart;
    }

    void Update()
    {
        if (player == null) return;

        float targetZ = player.position.z + spawnDistance;
        while (lastSpawnZ < targetZ)
        {
            SpawnRow(lastSpawnZ);
            lastSpawnZ += spawnInterval;
        }
    }

    void SpawnRow(float zPos)
    {
        // Lado izquierdo
        for (int i = 0; i < objectsPerSide; i++)
        {
            SpawnProp(-xOffset - i * sideSpacing, zPos);
        }

        // Lado derecho
        for (int i = 0; i < objectsPerSide; i++)
        {
            SpawnProp(xOffset + i * sideSpacing, zPos);
        }
    }

    void SpawnProp(float xPos, float zPos)
    {
        bool spawnTree = (Random.value > 0.3f); // 70% árbol, 30% arbusto
        GameObject prefab = spawnTree ?
            treePrefabs[Random.Range(0, treePrefabs.Length)] :
            bushPrefabs[Random.Range(0, bushPrefabs.Length)];

        Vector3 spawnPos = new Vector3(xPos, 0f, zPos);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
