using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public GameObject currentItem;

    private static GameObject tooltipPanel;
    private static TMP_Text tooltipNameText;
    private static TMP_Text tooltipDescriptionText;
    private static Image tooltipIconImage;

    private void Awake() {
        if (tooltipPanel == null) {
            tooltipPanel = GameObject.Find("ItemTooltip");
            if (tooltipPanel != null) {
                tooltipNameText        = tooltipPanel.transform.Find("ItemName")?.GetComponent<TMP_Text>();
                tooltipDescriptionText = tooltipPanel.transform.Find("ItemDescription")?.GetComponent<TMP_Text>();
                tooltipIconImage       = tooltipPanel.transform.Find("ItemIcon")?.GetComponent<Image>();

                Canvas canvas = tooltipPanel.GetComponentInParent<Canvas>();
                if (canvas != null) {
                    tooltipPanel.transform.SetParent(canvas.transform, true);
                    tooltipPanel.transform.SetAsLastSibling();
                }

                tooltipPanel.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (currentItem == null || tooltipPanel == null) return;

        Item item = currentItem.GetComponent<Item>();
        if (item == null) return;

        if (tooltipNameText != null)        tooltipNameText.text        = item.Name;
        if (tooltipDescriptionText != null) tooltipDescriptionText.text = item.description;

        if (tooltipIconImage != null) {
            Image icon = currentItem.GetComponent<Image>();
            tooltipIconImage.sprite  = icon != null ? icon.sprite : null;
            tooltipIconImage.enabled = tooltipIconImage.sprite != null;
        }

        tooltipPanel.transform.SetAsLastSibling();
        tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void Update() {
        if (tooltipPanel != null && tooltipPanel.activeSelf) {
			Vector2 mousePos = Mouse.current.position.ReadValue();
			tooltipPanel.transform.position = mousePos + new Vector2(250f, -100f);
		}
    }
}