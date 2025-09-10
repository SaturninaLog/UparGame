using UnityEngine;

public class CoinRotator : MonoBehaviour
{
    public float rotationSpeed = 100f;    // Velocidad de giro en grados por segundo
    public string modelChildName = "Bake_Moneda02";  // Nombre del hijo que rota

    private Transform model;

    void Start()
    {
        model = transform.Find(modelChildName);
        if (model == null)
        {
            Debug.LogWarning($"No se encontró el hijo {modelChildName} en {gameObject.name}");
        }
    }

    void Update()
    {
        if (model != null)
        {
            // 🔹 Rotación solo en el eje Z
            model.Rotate(0f, 0f, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
