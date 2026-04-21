using UnityEngine;
using UnityEngine.InputSystem;
 
public class Shop : MonoBehaviour, IInteractable {
    public GameObject shopUI;
    public ShopInventory shopInventory;
 
    void Start() {
        SetShopActive(false);
    }
 
    void Update() {
        if (shopUI.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame) {
            CloseShop();
        }
 
        if (!shopUI.activeSelf && Time.timeScale == 0f) {
            Time.timeScale = 1f;
        }
    }
 
    public bool IsOpen => shopUI != null && shopUI.activeSelf;
 
    public bool CanInteract() {
        return true;
    }
 
    public void Interact() {
        if (!CanInteract()) return;
        if (IsOpen)
            CloseShop();
        else
            OpenShop();
    }
 
    public void OpenShop() {
        SetShopActive(true);
        if (shopInventory != null) {
            shopInventory.PopulateShop();
        }
    }
 
    public void CloseShop() {
        SetShopActive(false);
    }
 
    private void SetShopActive(bool active) {
        shopUI.SetActive(active);
        Time.timeScale = active ? 0f : 1f;
    }
}