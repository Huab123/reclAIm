using UnityEngine;
using System.Collections.Generic;

public class ShopInventory : MonoBehaviour {
    public GameObject shopPanel;
    public GameObject shopSlotPrefab; // Prefab with ShopSlot component
    public List<ShopItem> shopItems;

    void Start() {
        PopulateShop();
    }

    public void PopulateShop() {
        // Clear existing slots
        foreach (Transform child in shopPanel.transform) {
            Destroy(child.gameObject);
        }

        // Instantiate slots with items
        foreach (ShopItem si in shopItems) {
            GameObject slotGO = Instantiate(shopSlotPrefab, shopPanel.transform);
            ShopSlot slot = slotGO.GetComponent<ShopSlot>();
            if (slot != null) {
                slot.price = si.price;
                if (slot.costText != null) slot.costText.text = si.price.ToString();
                GameObject itemGO = Instantiate(si.itemPrefab, slotGO.transform);
                itemGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = itemGO;

                // Disable dragging for shop items
                ItemDragHandler dragHandler = itemGO.GetComponent<ItemDragHandler>();
                if (dragHandler != null) {
                    dragHandler.enabled = false;
                }
            }
        }
    }
}

[System.Serializable]
public class ShopItem {
    public GameObject itemPrefab;
    public int price;
}