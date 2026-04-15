using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour {
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        interactionIcon.SetActive(false); 
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.performed) {
            if (interactableInRange == null) return;

            interactableInRange.Interact();

            if (!interactableInRange.CanInteract()) {
                interactionIcon.SetActive(false);
                interactableInRange = null;
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract()) {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    
    private void OnTriggerExit2D (Collider2D collision) {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange) {
            if (interactable is Shop shop && shop.IsOpen) {
                shop.CloseShop();
            }

            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
