using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {
	public int ID;
    public string Name;
    private bool pickedUp = false;

    public virtual void UseItem() {
        Debug.Log("Using item " + Name);
    }

    public virtual void Pickup() {
        if (pickedUp) return;
        pickedUp = true;
        
        Sprite itemIcon = GetComponent<Image>().sprite;
        ItemPickupUIController.Instance?.ShowItemPickup(Name, itemIcon);
    }
}
