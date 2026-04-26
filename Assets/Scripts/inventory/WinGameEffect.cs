using UnityEngine;

public class InventoryPassiveEffect : MonoBehaviour
{
    public string textObjectName = "MyTextName";

    private GameObject textUI;
    private bool isInInventory = false;

    void Start()
    {
        textUI = GameObject.Find(textObjectName);
        if (textUI != null) textUI.SetActive(false);
    }

    void Update()
    {
        bool nowInInventory = CheckIfInInventory();
        if (nowInInventory != isInInventory)
        {
            isInInventory = nowInInventory;
            if (textUI != null) textUI.SetActive(isInInventory);
        }
    }

    private bool CheckIfInInventory()
    {
        if (InventoryController.Instance == null) return false;
        foreach (Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                InventoryPassiveEffect effect = slot.currentItem.GetComponent<InventoryPassiveEffect>();
                if (effect != null && effect.textObjectName == textObjectName)
                    return true;
            }
        }
        return false;
    }
}