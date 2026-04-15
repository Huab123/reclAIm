using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int ID;
    public string itemName;
    public Sprite icon;

    public virtual void Use(GameObject user)
    {
        Debug.Log($"Using {itemName}");
    }
}