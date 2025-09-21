using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.PlayFX(AudioManager.instance.RecolectarSouvenirFX);
            CollectibleUIManager.instance.AddCollectible();
            Destroy(gameObject); // Destruye el objeto al recogerlo
        }
    }
}
