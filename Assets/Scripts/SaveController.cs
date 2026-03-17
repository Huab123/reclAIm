using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;

public class SaveController : MonoBehaviour {
	private string saveLocation;
    private InventoryController inventoryController;
	private HotbarController hotbarController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
       	saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json"); 
		inventoryController = FindFirstObjectByType<InventoryController>();
		hotbarController = FindFirstObjectByType<HotbarController>();

		LoadGame();
    }

    public void SaveGame() {
       	SaveData saveData = new SaveData {
			playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
			mapBoundary = FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
			inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),
		};

		File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

	public void LoadGame() {
		if (File.Exists(saveLocation)) {
			SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
		
			GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;

			PolygonCollider2D savedMapBoundary = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>(); 
			FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D = savedMapBoundary;

			MapController_Manual.Instance?.HighlightArea(saveData.mapBoundary); 
			MapController_Dynamic.Instance?.GenerateMap(savedMapBoundary);

			inventoryController.SetInventoryItems(saveData.inventorySaveData);
			hotbarController.SetHotbarItems(saveData.hotbarSaveData);
		}
		else {
			SaveGame();
            
			MapController_Dynamic.Instance?.GenerateMap();
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            hotbarController.SetHotbarItems(new List<InventorySaveData>());

		}
	}
}
