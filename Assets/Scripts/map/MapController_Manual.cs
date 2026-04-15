using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MapController_Manual : MonoBehaviour {
    public static MapController_Manual Instance { get; private set; }

    public GameObject mapParent;
    private List<Image> mapImages;

    public Color highlightColor = Color.yellow;
    public Color dimmedColor = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent white

    public RectTransform playerIconTransform;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Get all Image components in the map parent
        mapImages = mapParent.GetComponentsInChildren<Image>().ToList();
    }

    public void HighlightArea(string areaName) {
        foreach (Image area in mapImages) {
            area.color = dimmedColor;
        }

        Image currentArea = mapImages.Find(x => x.name == areaName);

        if (currentArea != null) {
            currentArea.color = highlightColor;

            playerIconTransform.position = currentArea.GetComponent<RectTransform>().position;
        }
        else {
            Debug.LogWarning($"Area '{areaName}' not found on the map.");
        
        }
    }
}
