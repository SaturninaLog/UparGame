using System.IO;
using UnityEngine;

public class PlayerProgressManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public float[] position;   // x, y, z
        public int health;
        public int score;
        public string[] inventory; // Ejemplo: nombres de ítems
    }

    public GameObject playerPrefab;    // Prefab del jugador
    private GameObject currentPlayer;  // Instancia del jugador

    private string saveFilePath;

    void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
    }

    void Start()
    {
        // Cargar datos al iniciar el juego (si existe)
        if (File.Exists(saveFilePath))
        {
            LoadPlayer();
        }
        else
        {
            // Si no existe, crear nuevo jugador
            currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    void Update()
    {
        // Guardar con G
        if (Input.GetKeyDown(KeyCode.G))
        {
            SavePlayer();
        }

        // Cargar con L
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayer();
        }
    }

    public void SavePlayer()
    {
        if (currentPlayer == null) return;

        PlayerData data = new PlayerData();

        // Guardar posición
        Vector3 pos = currentPlayer.transform.position;
        data.position = new float[3] { pos.x, pos.y, pos.z };

        // Aquí asignas datos según tu juego
        data.health = 100; // Ejemplo
        data.score = 500;  // Ejemplo
        data.inventory = new string[] { "Espada", "Llave" }; // Ejemplo

        // Serializar a JSON
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Progreso guardado en: " + saveFilePath);
    }

    public void LoadPlayer()
    {
        if (!File.Exists(saveFilePath)) return;

        // Leer JSON
        string json = File.ReadAllText(saveFilePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        // Si ya había un jugador, eliminarlo
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        // Instanciar jugador en la posición guardada
        Vector3 savedPos = new Vector3(data.position[0], data.position[1], data.position[2]);
        currentPlayer = Instantiate(playerPrefab, savedPos, Quaternion.identity);

        // Aquí podrías restaurar datos en componentes del jugador (vida, inventario, etc.)
        Debug.Log("Jugador cargado. Vida: " + data.health + ", Puntos: " + data.score);
    }
}
