using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int coins = 0;
    private int itemPieces = 0;
    public int piecesNeeded = 3;

    [Header("Nivel / Progreso")]
    public int coinsToNextLevel = 50;
    public string nextLevelName;

    [Header("UI Referencias")]
    public TMP_Text coinText;
    public TMP_Text pieceText;
    public Image itemImage;

    [Header("Souvenir UI")]
    public List<Image> souvenirImageDisplays;
    public List<Sprite> souvenirSprites;
    public Sprite souvenirLockedSprite;   // 🔥 sprite inicial (vacío/bloqueado)
    private int souvenirsCollected = 0;

    [Header("Multiplicador")]
    public int baseCoinValue = 1;
    public float multiplierDuration = 5f;
    private int currentMultiplier = 1;
    private float multiplierTimer = 0f;

    [Header("UI Multiplicador")]
    public TMP_Text multiplierText;

    public GroundManager groundManager;

    [Header("Fade")]
    public ScreenFader screenFader;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SetupUIReferences();
        ClearSouvenirsUI();   // 🟢 limpiar souvenirs al inicio
        UpdateUI();
        if (itemImage != null)
            itemImage.enabled = false;

        UpdateMultiplierUI();
    }

    private void Update()
    {
        if (currentMultiplier > 1)
        {
            multiplierTimer -= Time.deltaTime;
            if (multiplierTimer <= 0f)
            {
                currentMultiplier = 1;
                multiplierTimer = 0f;
            }
        }
        UpdateMultiplierUI();
    }

    private void UpdateMultiplierUI()
    {
        if (multiplierText != null)
        {
            multiplierText.text = currentMultiplier > 1 ? $"x{currentMultiplier}" : "";
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        coins = 0;
        itemPieces = 0;
        currentMultiplier = 1;
        multiplierTimer = 0f;

        SetupUIReferences();
        ClearSouvenirsUI();   // 🟢 limpiar souvenirs siempre al cargar escena
        UpdateUI();
        UpdateMultiplierUI();
    }

    // 🟢 Método centralizado para buscar la UI en cada escena
    private void SetupUIReferences()
    {
        GameObject coinTextObject = GameObject.Find("CoinText");
        if (coinTextObject != null)
            coinText = coinTextObject.GetComponent<TMP_Text>();

        GameObject pieceTextObject = GameObject.Find("PieceText");
        if (pieceTextObject != null)
            pieceText = pieceTextObject.GetComponent<TMP_Text>();

        GameObject multTextObj = GameObject.Find("MultiplierText");
        if (multTextObj != null)
            multiplierText = multTextObj.GetComponent<TMP_Text>();

        GameObject itemImageObj = GameObject.Find("ItemImage");
        if (itemImageObj != null)
            itemImage = itemImageObj.GetComponent<Image>();
    }

    // 🟢 función mejorada para limpiar souvenirs
    private void ClearSouvenirsUI()
    {
        souvenirsCollected = 0;

        if (souvenirImageDisplays == null)
            souvenirImageDisplays = new List<Image>();

        souvenirImageDisplays.Clear();
        for (int i = 0; i < souvenirSprites.Count; i++)
        {
            GameObject obj = GameObject.Find("Souvenir" + (i + 1));
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                // 🔥 en vez de null, le ponemos el sprite inicial bloqueado
                img.sprite = souvenirLockedSprite;
                img.gameObject.SetActive(true);
                souvenirImageDisplays.Add(img);
            }
        }

        Debug.Log("[GameManager] Souvenirs reseteados en la escena");
    }

    public void AddCoin(int amount)
    {
        int finalAmount = amount * currentMultiplier * baseCoinValue;
        coins += finalAmount;
        UpdateUI();

        if (coins >= coinsToNextLevel)
        {
            GoToNextLevel();
        }
    }

    public void AddItemPiece()
    {
        itemPieces++;
        UpdateUI();

        if (itemPieces >= piecesNeeded)
        {
            if (itemImage != null)
                itemImage.enabled = true;

            itemPieces = 0;
            UpdateUI();
        }

        ActivateMultiplier();
    }

    private void ActivateMultiplier()
    {
        souvenirsCollected++;
        ShowSouvenirImage();

        currentMultiplier++;
        multiplierTimer = multiplierDuration;

        Debug.Log($"Multiplicador activado: x{currentMultiplier} por {multiplierDuration} s");
    }

    public void ShowSouvenirImage()
    {
        if (souvenirsCollected <= souvenirSprites.Count)
        {
            int index = souvenirsCollected - 1;
            if (index >= 0 && index < souvenirImageDisplays.Count)
            {
                souvenirImageDisplays[index].sprite = souvenirSprites[index];
                souvenirImageDisplays[index].gameObject.SetActive(true);
            }
        }
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = $"{coins}";
        if (pieceText != null)
            pieceText.text = $"Piezas: {itemPieces}/{piecesNeeded}";
    }

    public void ResetScore()
    {
        coins = 0;
        currentMultiplier = 1;
        multiplierTimer = 0f;
        ClearSouvenirsUI();   // 🟢 limpiar souvenirs al reiniciar
        UpdateUI();
        UpdateMultiplierUI();
    }

    public void ResetMultiplierAndScore()
    {
        coins = 0;
        currentMultiplier = 1;
        multiplierTimer = 0f;
        ClearSouvenirsUI();   // 🟢 limpiar souvenirs también aquí
        UpdateUI();
    }

    private void GoToNextLevel()
    {
        Debug.Log("¡Monedas suficientes! Cargando siguiente nivel...");
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            if (screenFader != null)
            {
                StartCoroutine(screenFader.FadeOutIn(() =>
                {
                    SceneManager.LoadScene(nextLevelName);
                }));
            }
            else
            {
                SceneManager.LoadScene(nextLevelName);
            }
        }
        else
        {
            Debug.LogWarning("No se configuró el nombre del siguiente nivel en GameManager.");
        }
    }
}
