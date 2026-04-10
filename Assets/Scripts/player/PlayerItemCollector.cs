using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    void Start() {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Item")) {
            Item item = collision.GetComponent<Item>();
            if (item != null && !item.IsPickedUp) {
                Collider2D col = collision.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                ArmorItem armorItem = collision.GetComponent<ArmorItem>();
                bool itemAdded;

                if (armorItem != null) {
                    // Try to place into the matching armor slot
                    itemAdded = TryPickupArmor(armorItem, item);
                } else {
                    // Normal item — goes into inventory as before
                    itemAdded = inventoryController.AddItem(item.ID);
                }

                if (itemAdded) {
                    if (SoundEffectManager.Instance != null) {
                        SoundEffectManager.Instance.PlayPotionPickup();
                    }
                    item.Pickup();
                    Destroy(collision.gameObject);
                } else {
                    // No room — re-enable collider so it stays in the world
                    if (col != null) col.enabled = true;
                }
            }
        }
    }

    private bool TryPickupArmor(ArmorItem armorItem, Item item) {
        // Only pick up if the matching armor slot is empty
        ArmorController ac = ArmorController.Instance;
        if (ac == null) return false;

        // Check the correct slot for this armor type
        if (!ac.IsArmorSlotEmpty(armorItem.armorType)) {
            Debug.Log($"[Pickup] Armor slot {armorItem.armorType} is already occupied.");
            return false;
        }

        ac.EquipArmorDirectly(armorItem.armorType, item.gameObject);
        return true;
    }
}
