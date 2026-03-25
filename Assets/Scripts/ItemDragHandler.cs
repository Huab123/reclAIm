using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
	Transform originalParent;
	CanvasGroup canvasGroup;

	private InventoryController inventoryController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    	canvasGroup = GetComponent<CanvasGroup>();
		inventoryController = InventoryController.Instance;
    }

	public void OnBeginDrag(PointerEventData eventData) {
		originalParent = transform.parent;
		transform.SetParent(transform.root);	
		if (canvasGroup != null) {
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0.6f;
		}
	}

	public void OnDrag(PointerEventData eventData) {
		transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData) {
		if (canvasGroup != null) {
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1f;
		}

		Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();

		if (dropSlot == null) {
			GameObject dropItem = eventData.pointerEnter;
			if (dropItem != null) {
				dropSlot = dropItem.GetComponentInParent<Slot>();
			}
		}

		Slot originalSlot = originalParent.GetComponent<Slot>();
		bool cancelMove = false;

		if (dropSlot != null) {
			// If the drop slot already has an item, swap them or stack if possible
			if (dropSlot.currentItem != null) {
				Item draggedItem = GetComponent<Item>();
				Item targetItem = dropSlot.currentItem.GetComponent<Item>();

				if (draggedItem.ID == targetItem.ID) {
					if (targetItem.quantity < Item.MAX_STACK_SIZE) {
						int canAdd = Item.MAX_STACK_SIZE - targetItem.quantity;
						int toAdd = Mathf.Min(draggedItem.quantity, canAdd);
						targetItem.AddToStack(toAdd);

						if (toAdd >= draggedItem.quantity) {
							// full transfer
							originalSlot.currentItem = null;
							Destroy(gameObject);
							return;
						} else {
						// partial transfer; remainder goes back to original slot
						draggedItem.RemoveFromStack(toAdd);
						transform.SetParent(originalParent);
						originalSlot.currentItem = gameObject;
						GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
						return;
					}
					} else {
					// target full: return whole dragged stack to origin
					cancelMove = true;
					transform.SetParent(originalParent);
					originalSlot.currentItem = gameObject;
					GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
					return;
				}
				} else {
					dropSlot.currentItem.transform.SetParent(originalSlot.transform);
					originalSlot.currentItem = dropSlot.currentItem;
					dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				}
			} else {
				originalSlot.currentItem = null;
			}

			// Place the dragged item in the new slot if not destroyed and not canceled
			if (!cancelMove && gameObject != null && transform.parent != dropSlot.transform) {
				transform.SetParent(dropSlot.transform);
				dropSlot.currentItem = gameObject;
			}
		}
		else {
			if(!IsWithinInventory(eventData.position)) {
				// Drop the item outside the inventory (e.g., into the world)
				DropItem(originalSlot, eventData.position);
			}
			else {
				// Return to original slot if dropped outside any slot but within inventory
				transform.SetParent(originalParent);
			}
		}

		GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

	}

	bool IsWithinInventory(Vector2 mousePosition) {
		RectTransform inventoryRect = originalParent.GetComponentInParent<RectTransform>();
		return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
	}

	void DropItem(Slot originalSlot, Vector2 screenPosition){
		Item item = GetComponent<Item>();
		int quantity = item.quantity;

		// Drop the whole stack out of inventory
		originalSlot.currentItem = null;

		// Convert screen position to world point on Z=0 plane
		Camera cam = Camera.main;
		Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane));
		worldPos.z = 0f; // ensure 2D plane

		// Instantiate the dropped item objects in the world for each quantity unit
		for (int i = 0; i < Mathf.Max(quantity, 1); i++) {
			Vector3 dropOffset = worldPos + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f);

			GameObject dropItem = Instantiate(gameObject, dropOffset, Quaternion.identity);

			// Prevent clones-of-clones by stripping inventory drag behavior on world items
			ItemDragHandler dragHandler = dropItem.GetComponent<ItemDragHandler>();
			if (dragHandler != null) {
				Destroy(dragHandler);
			}

			CanvasGroup canvasGroup = dropItem.GetComponent<CanvasGroup>();
			if (canvasGroup != null) {
				Destroy(canvasGroup);
			}

			Item droppedItem = dropItem.GetComponent<Item>();
			if (droppedItem != null) {
				droppedItem.quantity = 1;
				droppedItem.UpdateQuantityDisplay();
			}

			BounceEffect bounce = dropItem.GetComponent<BounceEffect>();
			if (bounce != null) bounce.StartBounce();
		}

		// Destroy the inventory UI item
		Destroy(gameObject);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			SplitStack();
		}
	}

	private void SplitStack()
	{
		Item item = GetComponent<Item>();
		if (item == null || item.quantity <= 1) return;

		int splitAmount = item.quantity / 2;
		if (splitAmount <= 0) return;

		item.RemoveFromStack(splitAmount);

		GameObject newItem = item.CloneItem(splitAmount);

		if (inventoryController == null || newItem == null) return;

		foreach(Transform slotTransform in inventoryController.inventoryPanel.transform)
		{
			Slot slot = slotTransform.GetComponent<Slot>();
			if (slot != null && slot.currentItem == null)
			{
				slot.currentItem = newItem;
				newItem.transform.SetParent(slot.transform);
				newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				return;
			}
		}

		// No empty slot - return to stack
		item.AddToStack(splitAmount);
		Destroy(newItem);
	}
}
