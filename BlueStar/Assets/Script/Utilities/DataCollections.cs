using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string name;
    public ItemType itemType;
    public Sprite itemIcon;
    public GameObject itemObject;
    //public int itemAmount = 1;
    public string itemDescriptions;
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    
}

[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}
