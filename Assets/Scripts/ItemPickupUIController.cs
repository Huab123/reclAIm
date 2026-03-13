using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ItemPickupUIController : MonoBehaviour {
    public static ItemPickupUIController Instance { get; private set; }

    public GameObject popupPrefab;
    public int maxPopups = 4;
    public float popupDuration = 3f;

    private readonly Queue<GameObject>  activePopups = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } 
        else {
            Debug.LogError("Multiple ItemPickupUIManager instances detected! Destroying the exxtra one.");
            Destroy(gameObject);
        }
    }

    public void ShowItemPickup(string itemName, Sprite itemIcon) {
        GameObject newPopup = Instantiate(popupPrefab, transform);
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName;

        Image itemImage = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage) {
            itemImage.sprite = itemIcon;
        } 

        activePopups.Enqueue(newPopup);
        if (activePopups.Count > maxPopups) {
            Destroy(activePopups.Dequeue());
        }

        // Fade out and Destroy
        StartCoroutine(FadeOutAndDestroy(newPopup));
    }


    private IEnumerator FadeOutAndDestroy(GameObject popup) {
        yield return new WaitForSeconds(popupDuration);
        if (popup == null) yield break;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        for (float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime) {
            if (popup == null) yield break;
            canvasGroup.alpha = 1f - timePassed;
            yield return null;
        }

        Destroy(popup);
    }
}
