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

[System.Serializable]
public class EmitterDetails
{
    public int ID;
    public string name;
    public Sprite icon;
    public GameObject model_Unlaunched;
    public GameObject model_launched;
    public GameObject model_Orbit;
    public float health;
    public int bulletLeft;
    //还要加上发射子弹的类型
}

//子弹的种类
public enum BulletType
{
    one_Dimension, two_Dimension,tree_Dimension,blackHole
}