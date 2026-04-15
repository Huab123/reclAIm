
using UnityEngine;
 
public class Shop : MonoBehaviour, IInteractable {
    public GameObject shopUI;
 
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
        if (shopUI != null)
            shopUI.SetActive(true);
    }
 
    public void CloseShop() {
        if (shopUI != null)
            shopUI.SetActive(false);
    }
}
 