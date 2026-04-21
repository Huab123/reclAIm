using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public GameObject currentItem;
    public int price;
    public TMP_Text costText;

    private static GameObject tooltipPanel;
    private static TMP_Text tooltipNameText;
    private static TMP_Text tooltipDescriptionText;
    private static Image tooltipIconImage;

    private void Awake() {
        costText = transform.Find("Cost Text")?.GetComponent<TMP_Text>();

        // Tooltip setup
        if (tooltipPanel == null) {
            tooltipPanel = GameObject.Find("ItemTooltip");
            if (tooltipPanel != null) {
                tooltipNameText        = tooltipPanel.transform.Find("ItemName")?.GetComponent<TMP_Text>();
                tooltipDescriptionText = tooltipPanel.transform.Find("ItemDescription")?.GetComponent<TMP_Text>();
                tooltipIconImage       = tooltipPanel.transform.Find("ItemIcon")?.GetComponent<Image>();

                Canvas canvas = tooltipPanel.GetComponentInParent<Canvas>();
                if (canvas != null) {
                    tooltipPanel.transform.SetParent(canvas.transform, true);
                    tooltipPanel.transform.SetAsLastSibling();
                }

                tooltipPanel.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (currentItem == null || tooltipPanel == null) return;

        Item item = currentItem.GetComponent<Item>();
        if (item == null) return;

        if (tooltipNameText != null)        tooltipNameText.text        = item.Name;
        if (tooltipDescriptionText != null) tooltipDescriptionText.text = item.description;

        if (tooltipIconImage != null) {
            Image icon = currentItem.GetComponent<Image>();
            tooltipIconImage.sprite  = icon != null ? icon.sprite : null;
            tooltipIconImage.enabled = tooltipIconImage.sprite != null;
        }

        tooltipPanel.transform.SetAsLastSibling();
        tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (currentItem == null) return;

        Item item = currentItem.GetComponent<Item>();
        if (item == null) return;

        if (PlayerStats.Instance.coins >= price) {
            bool success = InventoryController.Instance.AddItem(item.ID);
            if (success) {
                PlayerStats.Instance.coins -= price;
                // Update currency display
                PlayerMovement playerController = FindFirstObjectByType<PlayerMovement>();
                playerController?.UpdateCurrencyDisplay();

                Debug.Log($"Bought {item.Name} for {price} coins. Remaining coins: {PlayerStats.Instance.coins}");
            } else {
                Debug.Log("Failed to add item to inventory (inventory full?)");
            }
        } else {
            Debug.Log("Not enough coins to buy this item!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // Prevent dragging
    }

    public void OnDrag(PointerEventData eventData) {
        // Prevent dragging
    }

    public void OnEndDrag(PointerEventData eventData) {
        // Prevent dragging
    }

    private void Update() {
        if (tooltipPanel != null && tooltipPanel.activeSelf) {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            tooltipPanel.transform.position = mousePos + new Vector2(250f, -100f);
        }
    }
}