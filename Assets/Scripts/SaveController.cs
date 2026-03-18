using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;
using System.Linq;

public class SaveController : MonoBehaviour {
	private string saveLocation;
    private InventoryController inventoryController;
	private HotbarController hotbarController;
	private Chest[] chests;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
		InitializeComponents();
		LoadGame();
    }

	private void InitializeComponents() {
       	saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json"); 
		inventoryController = FindFirstObjectByType<InventoryController>();
		hotbarController = FindFirstObjectByType<HotbarController>();
		chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);	
	}

    public void SaveGame() {
       	SaveData saveData = new SaveData {
			playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
			mapBoundary = FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
			inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),
			chestSaveData = GetChestsState(),
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

			LoadChestsState(saveData.chestSaveData);
		}
		else {
			SaveGame();
            
			MapController_Dynamic.Instance?.GenerateMap();
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            hotbarController.SetHotbarItems(new List<InventorySaveData>());

		}
	}

	private List<ChestSaveData> GetChestsState() {
		List<ChestSaveData> chestStates = new List<ChestSaveData>();
		foreach(Chest chest in chests) {
			ChestSaveData chestSaveData = new ChestSaveData {
				chestID = chest.ChestID,
				isOpened = chest.IsOpened,
			};
			chestStates.Add(chestSaveData);
		}

		return chestStates;
	}

	private void LoadChestsState(List<ChestSaveData> chestStates) {
		foreach(Chest chest in chests) {
			ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);

			if (chestSaveData != null)
			{
				chest.SetOpened(chestSaveData.isOpened);
			}
		}
	}
}
