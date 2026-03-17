using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapController_Dynamic : MonoBehaviour {
    [Header("UI References")]
    public RectTransform mapParent;
    public GameObject areaPrefab;
    public RectTransform playerIcon;

    [Header("Colors")]
    public Color defaultColor = Color.gray; // Areas on the map we aren't in
    public Color currentAreaColor = Color.green; // Active area

    [Header("Map Settings")]
    public GameObject mapBounds;
    public PolygonCollider2D initialArea;
    public float mapScale = 10f;
    private PolygonCollider2D[] mapAreas;
    private Dictionary<string, RectTransform> uiAreas = new Dictionary<string, RectTransform>(); // Map each PolygonCollider2D to corresponding RectTransform

    public static MapController_Dynamic Instance {get; set;}

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        mapAreas = mapBounds.GetComponentsInChildren<PolygonCollider2D>();
    }

    // Generate map
    public void GenerateMap(PolygonCollider2D newCurrentArea = null) {
        PolygonCollider2D currentArea = newCurrentArea != null ? newCurrentArea : initialArea;

        ClearMap();
        foreach(PolygonCollider2D area in mapAreas) {
            CreateAreaUI(area, area == currentArea);
        }

        MovePlayerIcon(currentArea.name);
    }

    // Clear map
    private void ClearMap() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
        uiAreas.Clear();
    }
    
    private void CreateAreaUI(PolygonCollider2D area, bool isCurrent) {
        // Instantiate prefab for image
        GameObject areaImage = Instantiate(areaPrefab, mapParent);
        RectTransform rectTransform = areaImage.GetComponent<RectTransform>();

        // Get bounds
        Bounds bounds = area.bounds;

        // Scale UI image fit map and bounds
        rectTransform.sizeDelta = new Vector2(bounds.size.x * mapScale, bounds.size.y * mapScale);
        rectTransform.anchoredPosition = bounds.center * mapScale;

        // Set color based on current or not
        areaImage.GetComponent<Image>().color = isCurrent ? currentAreaColor : defaultColor;

        // Add to dictionary
        uiAreas[area.name] = rectTransform;
    }

    public void UpdateCurrentArea(string newCurrentArea)
    {
        // Update Color
        foreach(KeyValuePair<string, RectTransform> area in uiAreas) {
            area.Value.GetComponent<Image>().color = area.Key == newCurrentArea ? currentAreaColor : defaultColor;
        }

        MovePlayerIcon(newCurrentArea);
    }

    private void MovePlayerIcon(string newCurrentArea) {
        if (uiAreas.TryGetValue(newCurrentArea, out RectTransform areaUI)) {
            // If current area was found set the icon position to center of area
            playerIcon.anchoredPosition = areaUI.anchoredPosition;
        }
    }

}