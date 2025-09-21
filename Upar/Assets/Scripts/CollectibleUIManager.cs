using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectibleUIManager : MonoBehaviour
{
    public static CollectibleUIManager instance;

    [Header("UI")]
    public TextMeshProUGUI collectibleText; // Texto que muestra el contador
    public Image rewardImage;              // Imagen que aparecerá al completar

    [Header("Contador de objetos")]
    public int collectedCount = 0;
    public int totalNeeded = 3; // Objetos necesarios para completar

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateUI();

        // 🔹 Ocultamos la imagen al inicio
        if (rewardImage != null)
            rewardImage.gameObject.SetActive(false);
    }

    public void AddCollectible()
    {
        collectedCount++;
        UpdateUI();

        if (collectedCount >= totalNeeded)
        {
            Debug.Log("¡Has recolectado los 3 sombreros!");

            // 🔹 Mostramos la imagen de recompensa
            if (rewardImage != null)
                rewardImage.gameObject.SetActive(true);
        }
    }

    private void UpdateUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = "Piezas: " + collectedCount + " / " + totalNeeded;
        }
    }
}
