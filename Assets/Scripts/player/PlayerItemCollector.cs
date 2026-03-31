using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
	private InventoryController inventoryController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
       	inventoryController = FindFirstObjectByType<InventoryController>(); 
    }

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.CompareTag("Item")) {
			Item item = collision.GetComponent<Item>();
			if (item != null && !item.IsPickedUp) {
				// Avoid double-collect on multi-collider/player-overlap
				Collider2D col = collision.GetComponent<Collider2D>();
				if (col != null) col.enabled = false;

				bool itemAdded = inventoryController.AddItem(item.ID);
				if (itemAdded) {
					if (SoundEffectManager.Instance != null)
                    {
                        SoundEffectManager.Instance.PlayPotionPickup();
                    }	
					item.Pickup();
					Destroy(collision.gameObject);
				} else {
					if (col != null) col.enabled = true;
				}
			}
		}
	}
}
