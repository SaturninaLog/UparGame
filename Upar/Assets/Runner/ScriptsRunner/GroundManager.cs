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
    public float souvenirYOffset = 1.5f;   // 🆕 Souvenir más elevado

    [Header("Run Settings")]
    public float laneDistance = 3f;
    public float groundLength = 30f;
    public int numberOfTiles = 6;

    [Header("Obstacle Spawn")]
    public float spawnMarginZ = 5f;
    public Vector2 obstaclesPerTile = new Vector2(2, 4);
    public float airObstacleYOffset = 1.5f;

    [Header("Background Props")]
    public GameObject[] backgroundProps2;     // Árboles / arbustos
    public float backgroundXOffset2 = 12f;
    public GameObject[] backgroundProps;
    public float backgroundXOffset = 12f;
    public float backgroundY = 0f;

    [Header("Souvenirs Únicos por Nivel")]
    public int totalSouvenirsInLevel = 3;
    private int souvenirsSpawned = 0;

    public float difficultyIncrement = 0.01f;
    private int tilesSinceLastSouvenir = 0;
    private int souvenirCounter = 0;

    private readonly List<GameObject> activeTiles = new List<GameObject>();
    private float zSpawn = 0f;
    private float currentSpawnMarginZ;

    void Start()
    {
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
        souvenirCounter = 0;
        souvenirsSpawned = 0;

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
        SpawnContentOnTile(tile, zSpawn);
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

    private void SpawnContentOnTile(GameObject parentTile, float tileZStart)
    {
        // 🟢 Souvenirs únicos por nivel
        if (souvenirsSpawned < totalSouvenirsInLevel && souvenirPrefabs.Length > 0)
        {
            if (Random.value < 0.15f) // probabilidad ajustable
            {
                int souvenirIndex = souvenirsSpawned % souvenirPrefabs.Length;
                GameObject selectedPrefab = souvenirPrefabs[souvenirIndex];
                if (selectedPrefab != null)
                {
                    GameObject souvenirInstance = Instantiate(selectedPrefab);

                    int lane = Random.Range(-1, 2);
                    float xPos = lane * laneDistance;
                    float zPos = tileZStart + Random.Range(currentSpawnMarginZ, groundLength - currentSpawnMarginZ);

                    // 🆕 ahora usamos souvenirYOffset
                    souvenirInstance.transform.position = new Vector3(xPos, souvenirYOffset, zPos);
                    souvenirInstance.transform.SetParent(parentTile.transform);

                    souvenirsSpawned++;
                }
            }
        }

        // 🟢 Obstáculos / monedas
        float totalChance = obstacleSpawnChance + coinSpawnChance;
        if (Random.value <= totalChance)
        {
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
                GameObject obj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
                obj.transform.SetParent(parentTile.transform);
            }
        }

        // 🟢 Background Props (árboles / arbustos)
        if (backgroundProps != null && backgroundProps.Length > 0)
        {
            float zBase = tileZStart + groundLength * 0.5f;

            GameObject leftPrefab = backgroundProps[Random.Range(0, backgroundProps.Length)];
            GameObject rightPrefab = backgroundProps[Random.Range(0, backgroundProps.Length)];

            GameObject leftInstance = Instantiate(leftPrefab, new Vector3(-backgroundXOffset, backgroundY, zBase), leftPrefab.transform.rotation);
            GameObject rightInstance = Instantiate(rightPrefab, new Vector3(backgroundXOffset, backgroundY, zBase), Quaternion.Euler(0, 180, 0));

            leftInstance.transform.SetParent(parentTile.transform);
            rightInstance.transform.SetParent(parentTile.transform);
        }

        // 🟢 Background Props 2
        if (backgroundProps2 != null && backgroundProps2.Length > 0)
        {
            float zBase = tileZStart + groundLength * 2f;

            GameObject leftPrefab = backgroundProps2[Random.Range(0, backgroundProps2.Length)];
            GameObject rightPrefab = backgroundProps2[Random.Range(0, backgroundProps2.Length)];

            GameObject leftInstance = Instantiate(leftPrefab, new Vector3(-backgroundXOffset2, backgroundY, zBase), leftPrefab.transform.rotation);
            GameObject rightInstance = Instantiate(rightPrefab, new Vector3(backgroundXOffset2, backgroundY, zBase), Quaternion.Euler(0, 180, 0));

            leftInstance.transform.SetParent(parentTile.transform);
            rightInstance.transform.SetParent(parentTile.transform);
        }
    }
}
