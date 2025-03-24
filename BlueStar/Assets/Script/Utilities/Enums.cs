public enum ItemType
{
    bulletKill,bulletFreeze,drug,battery,flashlight,card,tool,weapon,device,other
    //tool指的是钳子等辅助交互的工具，other指的是不需要交互，单纯收集的物品
}

public enum SlotType
{
    Bag,Box
}

public enum InventoryLocation
{
    Player,Box
}

public enum InteractionMode
{
    Trigger,Click
}