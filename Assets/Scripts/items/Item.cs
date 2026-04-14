using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class Item : MonoBehaviour {
    public const int MAX_STACK_SIZE = 10;
    public int ID;
    public string Name;
    [TextArea] public string description;
    private bool pickedUp = false;
    public bool IsPickedUp => pickedUp;
    public int quantity = 1;
    private TMP_Text quantityText;
    public ItemData data;

    private void Awake() {
        quantityText = transform.Find("Qty Text")?.GetComponent<TMP_Text>();
    }

    public void UpdateQuantityDisplay() {
        if (quantityText == null) {
            quantityText = transform.Find("Qty Text")?.GetComponent<TMP_Text>();
        }

        if (quantityText != null) {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1) {
        quantity = Mathf.Min(quantity + amount, MAX_STACK_SIZE);
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1) {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity) {
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
                playerController.Heal(20);
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
}