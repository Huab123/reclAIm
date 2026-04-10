using UnityEngine;
using System.Collections.Generic;


public enum ArmorSlotType { Helmet, Chestplate, Leggings, Boots }

public class ArmorController : MonoBehaviour
{
    public static ArmorController Instance { get; private set; }

    [Header("Assign your 4 armor slot GameObjects here")]
    public GameObject helmetSlotGO;
    public GameObject chestplateSlotGO;
    public GameObject leggingsSlotGO;
    public GameObject bootsSlotGO;

    private Dictionary<ArmorSlotType, Slot>      _armorSlots = new();
    private Dictionary<ArmorSlotType, ArmorSlot>  _armorMeta  = new();
    private Dictionary<ArmorSlotType, ArmorItem>  _equipped   = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        RegisterSlot(helmetSlotGO,     ArmorSlotType.Helmet);
        RegisterSlot(chestplateSlotGO, ArmorSlotType.Chestplate);
        RegisterSlot(leggingsSlotGO,   ArmorSlotType.Leggings);
        RegisterSlot(bootsSlotGO,      ArmorSlotType.Boots);
    }

    private void RegisterSlot(GameObject go, ArmorSlotType type)
    {
        if (go == null) { Debug.LogWarning($"[ArmorController] {type} slot not assigned in Inspector!"); return; }

        Slot slot = go.GetComponent<Slot>() ?? go.AddComponent<Slot>();
        ArmorSlot meta = go.GetComponent<ArmorSlot>() ?? go.AddComponent<ArmorSlot>();
        meta.acceptedType = type;

        _armorSlots[type] = slot;
        _armorMeta[type]  = meta;
    }


    public bool CanDropIntoSlot(GameObject draggedItemGO, Slot targetSlot)
    {
        ArmorItem armorItem = draggedItemGO.GetComponent<ArmorItem>();
        ArmorSlot armorSlot = targetSlot.GetComponent<ArmorSlot>();

        if (armorSlot != null)
            return armorItem != null && armorItem.armorType == armorSlot.acceptedType;

        if (armorItem != null)
            return false;

        return true;
    }


    public void OnArmorEquipped(ArmorItem armorItem, ArmorSlotType slotType)
    {
        if (_equipped.TryGetValue(slotType, out ArmorItem old) && old != null)
            RemoveArmorStats(old);

        _equipped[slotType] = armorItem;
        ApplyArmorStats(armorItem);
        Debug.Log($"[Armor] Equipped '{armorItem.name}' → {slotType}");
    }

    public void OnArmorUnequipped(ArmorSlotType slotType)
    {
        if (_equipped.TryGetValue(slotType, out ArmorItem old) && old != null)
        {
            RemoveArmorStats(old);
            _equipped[slotType] = null;
            Debug.Log($"[Armor] Unequipped from {slotType}");
        }
    }


    private void ApplyArmorStats(ArmorItem a)
    {
        PlayerStats s = PlayerStats.Instance;
        if (s == null) return;

        s.maxHealth       += a.bonusMaxHealth;
        s.moveSpeed       += a.bonusMoveSpeed;
        s.damageMult      += a.bonusDamageMult;
        s.attackSpeedMult += a.bonusAttackSpeedMult;
        s.critChance      += a.bonusCritChance;
        s.critDamageMult  += a.bonusCritDamageMult;
        s.reloadTime      -= a.bonusReloadTimeReduce;
        s.MaxAmmo         += a.bonusMaxAmmo;

        s.reloadTime = Mathf.Max(s.reloadTime, 0.05f);
        s.health     = Mathf.Min(s.health, s.maxHealth);

        RefreshHealthDisplay();
    }

    private void RemoveArmorStats(ArmorItem a)
    {
        PlayerStats s = PlayerStats.Instance;
        if (s == null) return;

        s.maxHealth       -= a.bonusMaxHealth;
        s.moveSpeed       -= a.bonusMoveSpeed;
        s.damageMult      -= a.bonusDamageMult;
        s.attackSpeedMult -= a.bonusAttackSpeedMult;
        s.critChance      -= a.bonusCritChance;
        s.critDamageMult  -= a.bonusCritDamageMult;
        s.reloadTime      += a.bonusReloadTimeReduce;
        s.MaxAmmo         -= a.bonusMaxAmmo;

        s.reloadTime = Mathf.Max(s.reloadTime, 0.05f);
        s.health     = Mathf.Min(s.health, s.maxHealth);

        RefreshHealthDisplay();
    }

    private void RefreshHealthDisplay()
    {
        PlayerMovement pm = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();
        pm?.UpdateHealthDisplay();
    }

    public bool IsArmorSlotEmpty(ArmorSlotType type)
    {
        return _armorSlots.TryGetValue(type, out Slot slot) && slot.currentItem == null;
    }

    public void EquipArmorDirectly(ArmorSlotType type, GameObject worldItemGO)
    {
        if (!_armorSlots.TryGetValue(type, out Slot slot)) return;

        Item worldItem = worldItemGO.GetComponent<Item>();
        ArmorItem worldArmorItem = worldItemGO.GetComponent<ArmorItem>();
        if (worldItem == null || worldArmorItem == null) return;

        ItemDictionary dict = FindFirstObjectByType<ItemDictionary>();
        if (dict == null) return;

        GameObject prefab = dict.GetItemPrefab(worldItem.ID);
        if (prefab == null) return;

        GameObject uiItem = Instantiate(prefab, slot.transform);
        uiItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Item uiItemComp = uiItem.GetComponent<Item>();
        if (uiItemComp != null) { uiItemComp.quantity = worldItem.quantity; uiItemComp.UpdateQuantityDisplay(); }

        slot.currentItem = uiItem;

        ArmorItem uiArmorComp = uiItem.GetComponent<ArmorItem>();
        if (uiArmorComp != null) OnArmorEquipped(uiArmorComp, type);
    }


    public List<InventorySaveData> GetArmorSaveData()
    {
        var list = new List<InventorySaveData>();
        foreach (var kvp in _armorSlots)
        {
            Slot slot = kvp.Value;
            if (slot.currentItem == null) continue;
            Item item = slot.currentItem.GetComponent<Item>();
            if (item != null)
                list.Add(new InventorySaveData
                {
                    itemID    = item.ID,
                    slotIndex = (int)kvp.Key,
                    quantity  = item.quantity,
                });
        }
        return list;
    }

    public void SetArmorItems(List<InventorySaveData> saveData, ItemDictionary dict)
    {
        foreach (var kvp in _armorSlots)
        {
            if (kvp.Value.currentItem != null)
            {
                Destroy(kvp.Value.currentItem);
                kvp.Value.currentItem = null;
            }
        }

        foreach (InventorySaveData data in saveData)
        {
            ArmorSlotType type = (ArmorSlotType)data.slotIndex;
            if (!_armorSlots.TryGetValue(type, out Slot slot)) continue;

            GameObject prefab = dict.GetItemPrefab(data.itemID);
            if (prefab == null) continue;

            GameObject itemGO = Instantiate(prefab, slot.transform);
            itemGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            Item itemComp = itemGO.GetComponent<Item>();
            if (itemComp != null) { itemComp.quantity = data.quantity; itemComp.UpdateQuantityDisplay(); }

            slot.currentItem = itemGO;

            ArmorItem armorComp = itemGO.GetComponent<ArmorItem>();
            if (armorComp != null) OnArmorEquipped(armorComp, type);
        }
    }
}