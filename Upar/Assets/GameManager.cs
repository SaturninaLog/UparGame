using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int coins = 0;
    private int itemPieces = 0;
    public int piecesNeeded = 3;

    [Header("UI References")]
    public TMP_Text coinText;
    public TMP_Text pieceText;
    public Image itemImage;

    [Header("Souvenir UI")]
    public List<Image> souvenirImageDisplays;
    public List<Sprite> souvenirSprites;

    // 💡 CAMBIO: Contador para saber cuántos souvenirs se han recogido
    private int souvenirsCollected = 0;

    public GroundManager groundManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
        if (itemImage != null)
        {
            itemImage.enabled = false;
        }
        foreach (var display in souvenirImageDisplays)
        {
            if (display != null)
            {
                display.gameObject.SetActive(false);
            }
        }
        // 💡 CAMBIO: Reiniciamos el contador de souvenirs al inicio del juego
        souvenirsCollected = 0;
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

        GameObject itemImageObject = GameObject.Find("ItemImage");
        if (itemImageObject != null)
        {
            itemImage = itemImageObject.GetComponent<Image>();
        }

        souvenirImageDisplays.Clear();
        for (int i = 1; i <= 3; i++)
        {
            GameObject souvenirObject = GameObject.Find($"SouvenirImage{i}");
            if (souvenirObject != null)
            {
                souvenirImageDisplays.Add(souvenirObject.GetComponent<Image>());
            }
        }

        GameObject coinTextObject = GameObject.Find("CoinText");
        if (coinTextObject != null)
        {
            coinText = coinTextObject.GetComponent<TMP_Text>();
        }

        GameObject pieceTextObject = GameObject.Find("PieceText");
        if (pieceTextObject != null)
        {
            pieceText = pieceTextObject.GetComponent<TMP_Text>();
        }

        if (itemImage != null)
        {
            itemImage.enabled = false;
        }
        foreach (var display in souvenirImageDisplays)
        {
            if (display != null)
            {
                display.gameObject.SetActive(false);
            }
        }

        // 💡 CAMBIO: Reiniciamos el contador de souvenirs al cargar la escena
        souvenirsCollected = 0;

        UpdateUI();

        groundManager = FindObjectOfType<GroundManager>();
        if (groundManager != null)
        {
            groundManager.ResetGround();
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.ResetPlayer();
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    public void AddItemPiece()
    {
        itemPieces++;
        UpdateUI();

        if (itemPieces >= piecesNeeded)
        {
            if (itemImage != null)
            {
                itemImage.enabled = true;
            }
            itemPieces = 0;
            UpdateUI();
        }
    }

    // 💡 CAMBIO: El método ya no recibe el índice. Usa el contador interno.
    public void ShowSouvenirImage()
    {
        if (souvenirsCollected < souvenirSprites.Count)
        {
            souvenirImageDisplays[souvenirsCollected].sprite = souvenirSprites[souvenirsCollected];
            souvenirImageDisplays[souvenirsCollected].gameObject.SetActive(true);
            souvenirsCollected++; // Incrementamos el contador
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {coins}";
        }
        if (pieceText != null)
        {
            pieceText.text = $"Pieces: {itemPieces}/{piecesNeeded}";
        }
    }
}
