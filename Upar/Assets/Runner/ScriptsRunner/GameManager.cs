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

    [Header("UI Referencias")]
    public TMP_Text coinText;
    public TMP_Text pieceText;
    public Image itemImage;

    [Header("Souvenir UI")]
    public List<Image> souvenirImageDisplays;
    public List<Sprite> souvenirSprites;
    private int souvenirsCollected = 0;

    [Header("Multiplicador")]
    public int baseCoinValue = 1;           // Valor base por moneda
    public float multiplierDuration = 5f;   // Cuánto dura el multiplicador
    private int currentMultiplier = 1;
    private float multiplierTimer = 0f;

    [Header("UI Multiplicador")]
    public TMP_Text multiplierText;         // Asigna en el inspector

    public GroundManager groundManager;

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
        UpdateUI();
        if (itemImage != null)
            itemImage.enabled = false;

        foreach (var display in souvenirImageDisplays)
            if (display != null)
                display.gameObject.SetActive(false);

        souvenirsCollected = 0;

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
            if (currentMultiplier > 1)
            {
                multiplierText.text = $"Monedas x{currentMultiplier} ({multiplierTimer:F1}s)";
            }
            else
            {
                multiplierText.text = ""; // Limpia cuando no hay multiplicador activo
            }
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

        // reacoplar referencias UI si es necesario en la nueva escena
        GameObject coinTextObject = GameObject.Find("CoinText");
        if (coinTextObject != null)
            coinText = coinTextObject.GetComponent<TMP_Text>();

        GameObject pieceTextObject = GameObject.Find("PieceText");
        if (pieceTextObject != null)
            pieceText = pieceTextObject.GetComponent<TMP_Text>();

        GameObject multTextObj = GameObject.Find("MultiplierText");
        if (multTextObj != null)
            multiplierText = multTextObj.GetComponent<TMP_Text>();

        UpdateUI();
        UpdateMultiplierUI();
    }

    public void AddCoin(int amount)
    {
        int finalAmount = amount * currentMultiplier * baseCoinValue;
        coins += finalAmount;
        UpdateUI();
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
            coinText.text = $"Coins: {coins}";
        if (pieceText != null)
            pieceText.text = $"Pieces: {itemPieces}/{piecesNeeded}";
    }

    public void ResetScore()
    {
        coins = 0;
        currentMultiplier = 1;
        multiplierTimer = 0f;
        UpdateUI();
        UpdateMultiplierUI();
    }

    public void ResetMultiplierAndScore()
    {
        coins = 0;
        currentMultiplier = 1;
        multiplierTimer = 0f;
        UpdateUI();
    }




}
