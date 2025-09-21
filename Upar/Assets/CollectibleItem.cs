using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectibleUIManager.instance.AddCollectible();
            Destroy(gameObject); // Destruye el objeto al recogerlo
        }
    }
}
