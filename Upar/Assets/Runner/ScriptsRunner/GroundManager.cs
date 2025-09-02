using UnityEngine;
using System.Collections.Generic;

public class GroundManager : MonoBehaviour
{
    [Header("Prefabs & Refs")]
    public GameObject groundPrefab;
    public GameObject[] obstaclePrefabs;
    public GameObject[] airObstaclePrefabs;
    public Transform player;

    [Header("Collectible Spawn")]
    public GameObject[] coinPrefabs;
    public GameObject[] souvenirPrefabs;
    [Range(0f, 1f)] public float obstacleSpawnChance = 0.5f;
    [Range(0f, 1f)] public float coinSpawnChance = 0.3f;
    public int souvenirSpawnInterval = 10;
    public float collectibleYOffset = 0.5f;

    [Header("Run Settings")]
    public float laneDistance = 3f;
    public float groundLength = 30f;
    public int numberOfTiles = 6;

    [Header("Obstacle Spawn")]
    public float spawnMarginZ = 5f;
    public Vector2 obstaclesPerTile = new Vector2(2, 4);
    public float airObstacleYOffset = 1.5f;

    public float difficultyIncrement = 0.01f;
    private int tilesSinceLastSouvenir = 0;
    // 💡 CAMBIO: Contador para asignar índices únicos a los souvenirs
    private int souvenirCounter = 0;

    private readonly List<GameObject> activeTiles = new List<GameObject>();
    private float zSpawn = 0f;
    private float currentSpawnMarginZ;

    void Start()
    {
        // Busca el jugador automáticamente si no está asignado
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        ResetGround();
    }

    public void ResetGround()
    {
        foreach (GameObject tile in activeTiles)
        {
            Destroy(tile);
        }
        activeTiles.Clear();
        zSpawn = 0f;
        currentSpawnMarginZ = spawnMarginZ;
        tilesSinceLastSouvenir = 0;
        // 💡 CAMBIO: Reiniciar el contador de souvenirs
        souvenirCounter = 0;

        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (player != null)
        {
            if (currentSpawnMarginZ > 6f)
            {
                currentSpawnMarginZ -= difficultyIncrement * Time.deltaTime;
            }

            if (player.position.z - 35f > zSpawn - (numberOfTiles * groundLength))
            {
                SpawnTile();
                DeleteTile();
            }
        }
    }

    private void SpawnTile()
    {
        if (groundPrefab == null)
        {
            Debug.LogError("GroundPrefab no está asignado en el Inspector de GroundManager.");
            return;
        }
        GameObject tile = Instantiate(groundPrefab, new Vector3(0f, 0f, zSpawn), Quaternion.identity);
        activeTiles.Add(tile);
        SpawnContentOnTile(zSpawn);
        zSpawn += groundLength;
        tilesSinceLastSouvenir++;
    }

    private void DeleteTile()
    {
        if (activeTiles.Count > 0)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
    }

    private void SpawnContentOnTile(float tileZStart)
    {
        if (tilesSinceLastSouvenir >= souvenirSpawnInterval && souvenirPrefabs.Length > 0)
        {
            // 💡 CAMBIO: Usamos el contador para obtener el índice del souvenir
            int souvenirIndex = souvenirCounter % souvenirPrefabs.Length;
            GameObject selectedPrefab = souvenirPrefabs[souvenirIndex];
            if (selectedPrefab == null)
            {
                Debug.LogError("Prefab de souvenir no está asignado en el Inspector.");
                return;
            }
            GameObject souvenirInstance = Instantiate(selectedPrefab);
            souvenirInstance.transform.position = new Vector3(Random.Range(-1, 2) * laneDistance, collectibleYOffset, tileZStart + Random.Range(currentSpawnMarginZ, groundLength - currentSpawnMarginZ));

            Souvenir souvenirScript = souvenirInstance.GetComponent<Souvenir>();
            if (souvenirScript != null)
            {
                // 💡 CAMBIO: Esta línea fue eliminada. El GameManager se encarga de la lógica.
                // Ya no necesitamos asignar un índice al script Souvenir.
            }
            tilesSinceLastSouvenir = 0;
            return;
        }

        float totalChance = obstacleSpawnChance + coinSpawnChance;
        if (Random.value > totalChance) return;

        int count = Random.Range((int)obstaclesPerTile.x, (int)obstaclesPerTile.y + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject selectedPrefab;
            float yPos;

            if (Random.value < (coinSpawnChance / totalChance) && coinPrefabs.Length > 0)
            {
                selectedPrefab = coinPrefabs[Random.Range(0, coinPrefabs.Length)];
                yPos = collectibleYOffset;
            }
            else
            {
                bool isAirObstacle = Random.value < 0.5f && airObstaclePrefabs.Length > 0;

                if (isAirObstacle)
                {
                    selectedPrefab = airObstaclePrefabs[Random.Range(0, airObstaclePrefabs.Length)];
                    yPos = airObstacleYOffset;
                }
                else
                {
                    selectedPrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                    yPos = selectedPrefab.transform.localScale.y * 0.5f;
                }
            }

            if (selectedPrefab == null)
            {
                Debug.LogError("Prefab de obstáculo o coleccionable no está asignado en el Inspector.");
                continue;
            }

            int lane = Random.Range(0, 3);
            float xPos = (lane - 1) * laneDistance;
            float zPos = tileZStart + Random.Range(currentSpawnMarginZ, groundLength - currentSpawnMarginZ);

            Vector3 spawnPos = new Vector3(xPos, yPos, zPos);
            Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        }
    }
}
