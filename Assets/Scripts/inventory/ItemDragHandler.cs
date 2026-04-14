using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    Transform originalParent;
    CanvasGroup canvasGroup;
    private InventoryController inventoryController;

    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private const float DRAG_THRESHOLD = 8f;

    void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController = InventoryController.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        dragStartPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!isDragging) {
            if (Vector2.Distance(eventData.position, dragStartPosition) < DRAG_THRESHOLD) return;
            isDragging = true;
            originalParent = transform.parent;
            transform.SetParent(transform.root);
            if (canvasGroup != null) {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0.6f;
            }
        }

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!isDragging) return;
        isDragging = false;

        if (canvasGroup != null) {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        Slot dropSlot = FindSlotUnderPointer(eventData);

        if (dropSlot != null && !ArmorController.Instance.CanDropIntoSlot(gameObject, dropSlot)) {
            ReturnToOrigin();
            return;
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null) {
            HandleDropOnSlot(dropSlot, originalSlot);
        } else {
            if (!IsWithinInventory(eventData.position)) {
                DropItem(originalSlot, eventData.position);
            } else {
                ReturnToOrigin();
            }
        }

        if (this != null && gameObject != null) {
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    Slot FindSlotUnderPointer(PointerEventData eventData) {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results) {
            if (result.gameObject == gameObject) continue;
            Slot slot = result.gameObject.GetComponentInParent<Slot>();
            if (slot != null) return slot;
        }
        return null;
    }

    void HandleDropOnSlot(Slot dropSlot, Slot originalSlot) {
        if (dropSlot.currentItem != null) {
            Item draggedItem = GetComponent<Item>();
            Item targetItem = dropSlot.currentItem.GetComponent<Item>();

            if (draggedItem != null && targetItem != null && draggedItem.ID == targetItem.ID) {
                if (targetItem.quantity < Item.MAX_STACK_SIZE) {
                    int canAdd = Item.MAX_STACK_SIZE - targetItem.quantity;
                    int toAdd = Mathf.Min(draggedItem.quantity, canAdd);
                    targetItem.AddToStack(toAdd);

                    if (toAdd >= draggedItem.quantity) {
                        if (originalSlot != null) originalSlot.currentItem = null;
                        ArmorSlot leftArmorSlot = originalParent.GetComponent<ArmorSlot>();
                        if (leftArmorSlot != null)
                            ArmorController.Instance.OnArmorUnequipped(leftArmorSlot.acceptedType);
                        Destroy(gameObject);
                        return;
                    } else {
                        draggedItem.RemoveFromStack(toAdd);
                        ReturnToOrigin();
                        return;
                    }
                } else {
                    ReturnToOrigin();
                    return;
                }
            } else {
                if (originalSlot != null) {
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    originalSlot.currentItem = dropSlot.currentItem;
                } else {
                    dropSlot.currentItem.transform.SetParent(transform.root);
                }
            }
        } else {
            if (originalSlot != null) originalSlot.currentItem = null;
        }

        transform.SetParent(dropSlot.transform);
        dropSlot.currentItem = gameObject;

        ArmorSlot oldArmorSlot = originalParent.GetComponent<ArmorSlot>();
        ArmorSlot newArmorSlot = dropSlot.GetComponent<ArmorSlot>();

        if (oldArmorSlot != null)
            ArmorController.Instance.OnArmorUnequipped(oldArmorSlot.acceptedType);

        if (newArmorSlot != null) {
            ArmorItem armorItem = GetComponent<ArmorItem>();
            if (armorItem != null)
                ArmorController.Instance.OnArmorEquipped(armorItem, newArmorSlot.acceptedType);
        }
    }

    void ReturnToOrigin() {
        transform.SetParent(originalParent);
        Slot originalSlot = originalParent.GetComponent<Slot>();
        if (originalSlot != null) originalSlot.currentItem = gameObject;
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    bool IsWithinInventory(Vector2 mousePosition) {
        RectTransform inventoryRect = originalParent.GetComponentInParent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot, Vector2 screenPosition) {
        Item item = GetComponent<Item>();
        int quantity = item != null ? item.quantity : 1;

        ArmorSlot armorSlot = originalParent.GetComponent<ArmorSlot>();
        if (armorSlot != null)
            ArmorController.Instance.OnArmorUnequipped(armorSlot.acceptedType);

        if (originalSlot != null) originalSlot.currentItem = null;

        Camera cam = Camera.main;
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane));
        worldPos.z = 0f;

        for (int i = 0; i < Mathf.Max(quantity, 1); i++) {
            Vector3 dropOffset = worldPos + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f);
            GameObject dropItem = Instantiate(gameObject, dropOffset, Quaternion.identity);

            ItemDragHandler dragHandler = dropItem.GetComponent<ItemDragHandler>();
            if (dragHandler != null) Destroy(dragHandler);

            CanvasGroup cg = dropItem.GetComponent<CanvasGroup>();
            if (cg != null) Destroy(cg);

            Item droppedItem = dropItem.GetComponent<Item>();
            if (droppedItem != null) {
                droppedItem.quantity = 1;
                droppedItem.UpdateQuantityDisplay();
            }

            BounceEffect bounce = dropItem.GetComponent<BounceEffect>();
            if (bounce != null) bounce.StartBounce();
        }

        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            SplitStack();
        }
    }

    private void SplitStack() {
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1) return;
        if (GetComponent<ArmorItem>() != null) return;

        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0) return;

        item.RemoveFromStack(splitAmount);
        GameObject newItem = item.CloneItem(splitAmount);

        if (inventoryController == null || newItem == null) {
            item.AddToStack(splitAmount);
            return;
        }

        foreach (Transform slotTransform in inventoryController.inventoryPanel.transform) {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null) {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }

        item.AddToStack(splitAmount);
        Destroy(newItem);
    }
}