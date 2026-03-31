using UnityEngine;

[CreateAssetMenu(menuName = "Items/Health Potion")]
public class HealthPotionData : ItemData
{
    public float healAmount;

    public override void Use(GameObject user)
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("PlayerStats instance not found!");
            return;
        }

        PlayerStats.Instance.health += healAmount;

        if (PlayerStats.Instance.health > PlayerStats.Instance.maxHealth)
        {
            PlayerStats.Instance.health = PlayerStats.Instance.maxHealth;
        }
        Debug.Log($"Healed for {healAmount}");
    }
}