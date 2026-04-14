using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
	public GameObject menuCanvas;

	void Start() {
		SetMenuActive(false);
    }

    void Update() {
      if (Keyboard.current.escapeKey.wasPressedThisFrame) {
		ToggleMenu();
	}

		// If the menu is closed by another UI path, restore normal time scale.
		if (!menuCanvas.activeSelf && Time.timeScale == 0f) {
			Time.timeScale = 1f;
		}
    }

	public void ToggleMenu() {
		SetMenuActive(!menuCanvas.activeSelf);
	}

	public void OpenMenu() {
		SetMenuActive(true);
	}

	public void CloseMenu() {
		SetMenuActive(false);
	}

	private void SetMenuActive(bool active) {
		menuCanvas.SetActive(active);
		Time.timeScale = active ? 0f : 1f;
	}
}
