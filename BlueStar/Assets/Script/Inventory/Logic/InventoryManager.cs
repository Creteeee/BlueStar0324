using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace BlueStar.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("玩家背包UI")] [SerializeField] private GameObject bagUI;
        private bool bagOpened;
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;

        [Header("背包数据")] public InventoryBag_SO playerBag;
        [Header("2DUI的Canvas")] [SerializeField]
        private GameObject canvas;


        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            bagOpened = bagUI.activeInHierarchy;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OpenBagUI();
                //canvas.GetComponent<CanvasGroup>().alpha =bagOpened?1:0;
                //canvas.GetComponent<CanvasGroup>().interactable = bagOpened?true:false;
            }
        }

        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.ItemDetailsList.Find(i => i.itemID == ID);
        }

        public void AddItem(Item item, bool toDestroy)
        {
            if (toDestroy)
            {
                //先判断背包是否有该物品
                var index = GetItemIndexBag(item.itemID);
                AddItemIndex(item.itemID,index,item.itemAmount);
                //再判断是否有空位
                InventoryItem newItem = new InventoryItem();
                newItem.itemID = item.itemID;
                newItem.itemAmount = item.itemAmount;
                Debug.Log("拾取了物体，ID："+GetItemDetails(item.itemID).itemID+"Name:"+GetItemDetails(item.itemID).name);
                Destroy(item.gameObject);
                
                //更新背包UI
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            }
        }

        /// <summary>
        /// 使用后减少数量
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isUse"></param>
        public void UseItem(int itemID, bool isUse)
        {
            if (isUse)
            {
                var index = GetItemIndexBag(itemID);
                int currentAmount = playerBag.itemList[index].itemAmount;
                if (currentAmount > 0)
                {
                    var itemNew = new InventoryItem() { itemID = itemID, itemAmount = currentAmount-1 };
                    playerBag.itemList[index] = itemNew;
                    
                    //如果使用过后的物品数量变为0，则删除物品
                    int refreshedAmount = playerBag.itemList[index].itemAmount;
                    if (refreshedAmount==0)
                    {
                        DeleteItem(itemID);
                    }
                }
                else
                {
                    DeleteItem(itemID);
                }
                //更新背包UI
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
                
            }
        }
        /// <summary>
        /// 删除背包SO中对应index的物体
        /// </summary>
        /// <param name="itemID"></param>

        private void DeleteItem(int itemID)
        {
            var index = GetItemIndexBag(itemID);
            var itemNew = new InventoryItem() { itemID = 0, itemAmount = 0 };
            playerBag.itemList[index] = itemNew;
            EventHandler.CallResetEmptySlot(index);
        }
        
        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagOpacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                {
                    return true;
                }
            }

            return false;
        }
        
/// <summary>
/// 在有相同物体时使用，返回背包中物体对应的列表位置index
/// </summary>
/// <param name="ID"></param>
/// <returns></returns>
        public int GetItemIndexBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                {
                    return i;
                }
            }

            return -1;//-1代表没有相同的物品
        }
/// <summary>
/// 添加背包物品
/// </summary>
/// <param name="ID"></param>
/// <param name="index"></param>
/// <param name="amount"></param>
        private void AddItemIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagOpacity())//背包没这个物体但有空位
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
                //还有种情况没有相同的也没空位就不加
            }
            else if(index !=-1 )
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem() { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }

        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
            
        }

    }
    
    
    
}

