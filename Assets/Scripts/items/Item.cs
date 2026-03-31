using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour {
	public const int MAX_STACK_SIZE = 10;
	public int ID;
    public string Name;
    private bool pickedUp = false;
    public bool IsPickedUp => pickedUp;
    public int quantity = 1;
    private TMP_Text quantityText;
    public ItemData data;

    private void Awake() {
        quantityText = GetComponentInChildren<TMP_Text>();
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

    public void UseItem(GameObject user)
{
    if (data != null)
    {
        data.Use(user); // now this is the player
    }

    if (quantity > 1)
        RemoveFromStack(1);
    else
        Destroy(gameObject);
}

    public virtual void Pickup() {
        if (pickedUp) return;
        pickedUp = true;

        Image image = GetComponent<Image>();
        Sprite itemIcon = image != null ? image.sprite : null;
        ItemPickupUIController.Instance?.ShowItemPickup(Name, itemIcon);
    }
}
