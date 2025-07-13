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
        public static ItemDetails item;
        [Header("UI组件")] [SerializeField] private TMP_Text buttonName;
        public int handledItemID;
        public static int WeaponID;
        public static int DeviceID;
        public static int carriedID;

        public static int selectedSlotIndex;


        private void Awake()
        {
            //WeaponID = 0;
            DeviceID = 0;
            carriedID = 0;
        }

        private void Start()
        {
        }

        private void Update()
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            item = InventoryManager.Instance.GetItemDetails(SlotUI.selectedID);
            Debug.Log("你惦记了使用按钮，当前激活的ID为"+SlotUI.selectedID+"这个ID对应的名字是"+InventoryManager.Instance.GetItemDetails(SlotUI.selectedID).name+"它carried的情况是"+InventoryManager.Instance.GetItemDetails(SlotUI.selectedID).canCarried);

            if (SlotUI.selectedID!=0)
            {
         
                            switch (item.itemType)
                                {
                                    case ItemType.card:
          
                                        InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried = !InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried;
                                        ChangeButtomName(item);
                                        handledItemID = InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried == true ? item.itemID : 0;
                                        Debug.Log("目前手持的物品ID为："+InventoryUI.Instance.playerSlots[selectedSlotIndex].ItemDetails.itemID);
                                        carriedID=InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried == true ? item.itemID : 0;
                                        return;
                                    
                                    case ItemType.weapon:

                                        InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried= !InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried;
                                        ChangeButtomName(item);
                                        WeaponID = InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried == true ? item.itemID : 0;
                                        
                                        Debug.Log("目前武器的ID为："+WeaponID);
                                        return;
                                    
                                    case ItemType.drug:

                                        ChangeButtomName(item);
                                        InventoryManager.Instance.UseItem(item.itemID,true);
                                        //Debug.Log("当前使用的物品名称为："+item.itemID+"数量为"+ InventoryManager.Instance.playerBag.itemList[InventoryManager.Instance.GetItemIndexBag(item.itemID)].itemAmount);
                                        //呼叫给玩家增加血量
                                        EventHandler.CallRecoverHealth(40f);
                                        StartCoroutine(InventoryManager.Instance.HealthRecoverVFX(-3, 9.35f, 1));
                                        PostProcessingManager.Instance.ResetFocalLength();
                                        if (InventoryManager.Instance.playerBag
                                                .itemList[InventoryManager.Instance.GetItemIndexBag(item.itemID)].itemAmount == 0)
                                        {
                                            SlotUI.selectedID = 0;
                                        }
                                        return;
                                    case ItemType.tool:
  
                                        InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried = !InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried;
                                        ChangeButtomName(item);
                                        handledItemID = InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried == true ? item.itemID : 0;
                                        Debug.Log("目前手持的物品ID为："+InventoryUI.Instance.playerSlots[selectedSlotIndex].ItemDetails.itemID);
                                        carriedID=InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried == true ? item.itemID : 0;
                                        return;
                                    case ItemType.bulletFreeze:
                                        return;
                                }
            }
            
        }

        void ChangeButtomName(ItemDetails item)
        {
            switch (item.itemType)
            {
                case ItemType.card:
                    buttonName.text = InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried== true ? "取下" : "手持";
                    return;
                case ItemType.weapon:
                    buttonName.text = InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried == true ? "取下" : "装备";
                    return;
                case ItemType.drug:
                    buttonName.text = "使用";
                    return;
                    
            }
        }

        public void DropItem()
        {
            item = InventoryManager.Instance.GetItemDetails(SlotUI.selectedID);
            if (SlotUI.selectedID!=0)
            {
                switch (item.itemType)
                {
                    case ItemType.drug:
                        InventoryManager.Instance.UseItem(item.itemID,true);
                        return;
                    case ItemType.bulletFreeze:
                        InventoryManager.Instance.UseItem(item.itemID,true);
                        return;
                        
                }
                
            }
            
        }
        
        //用于第一次拾取后装备武器的方法
        public void carryWeapon(GameObject carryWeaponUI)
        {
            SlotUI.selectedID = 1003;
            WeaponID = 1003;
            item = InventoryManager.Instance.GetItemDetails(SlotUI.selectedID);
            InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried= !InventoryUI.Instance.playerSlots[selectedSlotIndex].canCarrried;
            ChangeButtomName(item);
            Debug.Log("目前武器的ID为："+WeaponID);
            Destroy(carryWeaponUI.gameObject);
            
            
        }

        public void notcarryWeapon(GameObject carryWeaponUI)
        {
            Destroy(carryWeaponUI.gameObject);
        }
        


    
    }
}

