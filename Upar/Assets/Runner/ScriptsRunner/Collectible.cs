using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType { Coin, ItemPiece }
    public CollectibleType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.PlayFX(AudioManager.instance.MonedaCollectedFX);
            if (type == CollectibleType.Coin)
                GameManager.instance.AddCoin(1);
            else if (type == CollectibleType.ItemPiece)
                GameManager.instance.AddItemPiece();

            Destroy(gameObject);
        }
    }
}
