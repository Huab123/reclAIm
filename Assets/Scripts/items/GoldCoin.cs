using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance.coins += coinValue;
            PlayerMovement playerController = FindFirstObjectByType<PlayerMovement>();
            playerController?.UpdateCurrencyDisplay();
            Destroy(gameObject);
        }
    }
}