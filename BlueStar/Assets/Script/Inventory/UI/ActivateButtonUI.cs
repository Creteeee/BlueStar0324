using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace BlueStar.Inventory
{
    public class ActivateButtonUI : MonoBehaviour,IPointerClickHandler
    {
        private ItemDetails item;
        [Header("UI组件")] [SerializeField] private TMP_Text buttonName;
        public int handledItemID;
        public static int WeaponID;
        public static int DeviceID;
        public static int carriedID;


        private void Awake()
        {
            WeaponID = 0;
            DeviceID = 0;
            carriedID = 0;
        }

        private void Update()
        {
            item = InventoryManager.Instance.GetItemDetails(SlotUI.selectedID);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (item.itemType)
            {
                case ItemType.card:
                    item.canCarried = !item.canCarried;
                    ChangeButtomName(item);
                    handledItemID = item.canCarried == true ? item.itemID : 0;
                    Debug.Log("目前手持的物品ID为："+handledItemID);
                    carriedID=item.canCarried == true ? item.itemID : 0;
                    return;
                
                case ItemType.weapon:
                    item.canCarried = !item.canCarried;
                    ChangeButtomName(item);
                    WeaponID = item.canCarried == true ? item.itemID : 0;
                    Debug.Log("目前武器的ID为："+WeaponID);
                    return;
                
                case ItemType.drug:
                    ChangeButtomName(item);
                    InventoryManager.Instance.UseItem(item.itemID,true);
                    Debug.Log("当前使用的物品名称为："+item.itemID+"数量为"+ InventoryManager.Instance.playerBag.itemList[InventoryManager.Instance.GetItemIndexBag(item.itemID)].itemAmount);
                    if (InventoryManager.Instance.playerBag
                            .itemList[InventoryManager.Instance.GetItemIndexBag(item.itemID)].itemAmount == 0)
                    {
                        SlotUI.selectedID = 0;
                    }
                    return;
            }
            
        }

        void ChangeButtomName(ItemDetails item)
        {
            switch (item.itemType)
            {
                case ItemType.card:
                    buttonName.text = item.canCarried == true ? "取下" : "手持";
                    return;
                case ItemType.weapon:
                    buttonName.text = item.canCarried == true ? "取下" : "装备";
                    return;
                case ItemType.drug:
                    buttonName.text = "使用";
                    return;
                    
            }
        }


    
    }
}

