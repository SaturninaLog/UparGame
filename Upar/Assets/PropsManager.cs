using UnityEngine;
using System.Collections.Generic;

public class PropSpawner : MonoBehaviour
{
    [Header("Prefabs de Props")]
    public GameObject[] leftSideProps;
    public GameObject[] rightSideProps;

    [Header("Referencias")]
    public Transform player;            // asigna el player o se busca por tag "Player"
    public Transform spawnAnchor;       // opcional: empty en el centro de la pista (si no, se usa player.x al Start)

    [Header("Comportamiento X")]
    public bool followPlayerX = false;  // si true: spawnea relativo a player.x. si false: usa spawnAnchor o baseX
    public float horizontalOffset = 6f; // distancia en X respecto al center

    [Header("Spawn Z / Timing")]
    public float spawnDistance = 100f;  // cuánto adelante se generan
    public float spawnInterval = 10f;   // separación entre filas (en Z)
    public float despawnDistanceBehind = 25f; // cuando props están más atrás se destruyen

    [Header("Altura / fallback")]
    public float defaultYOffset = 0f;   // si raycast falla

    [Header("Opciones visuales")]
    public bool flipRightSide = true;   // si true, rota 180° los props de la derecha
    public int maxActiveProps = 80;

    // Estado interno
    private float lastSpawnZ = 0f;
    private float baseX = 0f; // centro fijo si followPlayerX == false
    private List<GameObject> activeProps = new List<GameObject>();

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // baseX: si hay spawnAnchor usa su X; si no, usa la X inicial del player
        if (spawnAnchor != null)
            baseX = spawnAnchor.position.x;
        else if (player != null)
            baseX = player.position.x;
        else
            baseX = 0f;

        // Inicializar next spawn Z justo adelante
        lastSpawnZ = (player != null) ? player.position.z + spawnInterval : spawnInterval;
    }

    void Update()
    {
        if (player == null) return;

        float centerX = followPlayerX ? player.position.x : baseX;

        // En vez de while, solo spawnea una fila por frame como máximo
        if (lastSpawnZ < player.position.z + spawnDistance)
        {
            SpawnRow(lastSpawnZ, centerX);
            lastSpawnZ += spawnInterval;
        }

        CleanupProps();
    }

    void SpawnRow(float zPos, float centerX)
    {
        // calcular posiciones X para cada lado (basadas en centerX)
        float leftX = centerX - horizontalOffset;
        float rightX = centerX + horizontalOffset;

        // LADO IZQUIERDO
        if (leftSideProps != null && leftSideProps.Length > 0)
        {
            GameObject prefab = leftSideProps[Random.Range(0, leftSideProps.Length)];
            Vector3 spawnPos = new Vector3(leftX, 100f, zPos);
            spawnPos.y = GetGroundY(spawnPos, defaultYOffset);
            GameObject prop = Instantiate(prefab, spawnPos, prefab.transform.rotation);
            activeProps.Add(prop);
        }

        // LADO DERECHO
        if (rightSideProps != null && rightSideProps.Length > 0)
        {
            GameObject prefab = rightSideProps[Random.Range(0, rightSideProps.Length)];
            Vector3 spawnPos = new Vector3(rightX, 100f, zPos);
            spawnPos.y = GetGroundY(spawnPos, defaultYOffset);
            GameObject prop = Instantiate(prefab, spawnPos, prefab.transform.rotation);

            if (flipRightSide)
            {
                // rotar 180 en Y para que mire hacia el otro lado
                prop.transform.Rotate(0f, 180f, 0f);
            }

            activeProps.Add(prop);
        }

        // limitar cantidad de props activos
        if (activeProps.Count > maxActiveProps)
        {
            Destroy(activeProps[0]);
            activeProps.RemoveAt(0);
        }
    }

    float GetGroundY(Vector3 startPos, float fallbackY)
    {
        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 500f))
        {
            return hit.point.y;
        }
        return fallbackY;
    }

    void CleanupProps()
    {
        for (int i = activeProps.Count - 1; i >= 0; i--)
        {
            if (activeProps[i] == null) { activeProps.RemoveAt(i); continue; }
            if (player.position.z - activeProps[i].transform.position.z > despawnDistanceBehind)
            {
                Destroy(activeProps[i]);
                activeProps.RemoveAt(i);
            }
        }
    }

    // útil si quieres actualizar el anchor en runtime (ej: cambiar a otro centro)
    public void SetSpawnAnchor(Transform anchor)
    {
        spawnAnchor = anchor;
        if (spawnAnchor != null) baseX = spawnAnchor.position.x;
    }
}
