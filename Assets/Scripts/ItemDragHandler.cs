using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	Transform originalParent;
	CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    	canvasGroup = GetComponent<CanvasGroup>();    
    }

	public void OnBeginDrag(PointerEventData eventData) {
		originalParent = transform.parent;
		transform.SetParent(transform.root);	
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.6f;
	}

	public void OnDrag(PointerEventData eventData) {
		transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData) {
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;

		Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();

		if (dropSlot == null) {
			GameObject dropItem = eventData.pointerEnter;
			if (dropItem != null) {
				dropSlot = dropItem.GetComponentInParent<Slot>();
			}
		}

		Slot originalSlot = originalParent.GetComponent<Slot>();
		if (dropSlot != null) {
			// If the drop slot already has an item, swap them
			if (dropSlot.currentItem != null) {

				dropSlot.currentItem.transform.SetParent(originalSlot.transform);
				originalSlot.currentItem = dropSlot.currentItem;
				dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			}
			else {
				originalSlot.currentItem = null;
			}
			// Place the dragged item in the new slot
			transform.SetParent(dropSlot.transform);
			dropSlot.currentItem = gameObject;
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
		originalSlot.currentItem = null;

		// Convert screen position to world point on Z=0 plane
		Camera cam = Camera.main;
		Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane));
		worldPos.z = 0f; // ensure 2D plane

		// Instantiate the dropped item in the world
		GameObject dropItem = Instantiate(gameObject, worldPos, Quaternion.identity);
		dropItem.GetComponent<BounceEffect>().StartBounce();

		// Destroy UI item
		Destroy(gameObject);
	}
}
