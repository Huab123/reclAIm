using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; 
using TMPro;

public class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public const int MAX_STACK_SIZE = 10;
	public int ID;
    public string Name;
    [TextArea] public string description;
    private bool pickedUp = false;
    public bool IsPickedUp => pickedUp;
    public int quantity = 1;
    private TMP_Text quantityText;

    private static GameObject tooltipPanel;
    private static TMP_Text tooltipNameText;
    private static TMP_Text tooltipDescriptionText;
    private static Image tooltipIconImage; // optional

    private void Awake() {
        quantityText = GetComponentInChildren<TMP_Text>();

        if (tooltipPanel == null) {
            tooltipPanel = GameObject.Find("ItemTooltip"); // match this to your GameObject name
            if (tooltipPanel != null) {
                tooltipNameText        = tooltipPanel.transform.Find("ItemName")?.GetComponent<TMP_Text>();
                tooltipDescriptionText = tooltipPanel.transform.Find("ItemDescription")?.GetComponent<TMP_Text>();
                tooltipIconImage       = tooltipPanel.transform.Find("ItemIcon")?.GetComponent<Image>();

                // Reparent tooltip to the root of its canvas so it renders on top of everything
                Canvas canvas = tooltipPanel.GetComponentInParent<Canvas>();
                if (canvas != null) {
                    tooltipPanel.transform.SetParent(canvas.transform, true);
                    tooltipPanel.transform.SetAsLastSibling(); // last sibling renders on top
                }

                tooltipPanel.SetActive(false);
            }
        }
    }

    private void OnDisable() {
        // Hide tooltip if this item gets disabled or destroyed while hovered
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void OnDestroy() {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    public void UpdateQuantityDisplay() {
        if (quantityText == null) {
            quantityText = GetComponentInChildren<TMP_Text>();
        }

        if (quantityText != null) {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        quantity = Mathf.Min(quantity + amount, MAX_STACK_SIZE);
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDisplay();
        return clone;
    }

    public virtual void UseItem() {
        if (!PerformItemEffect()) {
            Debug.LogWarning($"Item ID {ID} ('{Name}') has no use action defined.");
            return;
        }

        if (quantity > 1) {
            RemoveFromStack(1);
        } else {
            Destroy(gameObject);
        }
    }

    protected virtual bool PerformItemEffect() {
        switch (ID) {
            case 1:
            case 2:
                return UseHealthPotion();
            // Add more cases here for other item IDs.
            default:
                return false;
        }
    }

    private bool UseHealthPotion() {
        Debug.Log($"Using health potion: {Name}");
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            PlayerMovement playerController = player.GetComponent<PlayerMovement>();
            if (playerController != null) {
                playerController.Heal(20); // Heal 20 HP
                return true;
            }
        }
        return true;
    }

    public virtual void Pickup() {
        if (pickedUp) return;
        pickedUp = true;

        Image image = GetComponent<Image>();
        Sprite itemIcon = image != null ? image.sprite : null;
        ItemPickupUIController.Instance?.ShowItemPickup(Name, itemIcon);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (tooltipPanel == null) return;

        if (tooltipNameText != null)        tooltipNameText.text        = Name;
        if (tooltipDescriptionText != null) tooltipDescriptionText.text = description;

        if (tooltipIconImage != null) {
            Image image = GetComponent<Image>();
            tooltipIconImage.sprite  = image != null ? image.sprite : null;
            tooltipIconImage.enabled = tooltipIconImage.sprite != null;
        }

        // Always re-force to top when showing so it stays above any newly opened panels
        tooltipPanel.transform.SetAsLastSibling();
        tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (tooltipPanel == null) return;
        tooltipPanel.SetActive(false);
    }

    private void Update() {
        if (tooltipPanel != null && tooltipPanel.activeSelf) {
            tooltipPanel.transform.position = Mouse.current.position.ReadValue();
        }
    }
}