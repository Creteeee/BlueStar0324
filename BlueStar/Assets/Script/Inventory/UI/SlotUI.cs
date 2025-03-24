using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlueStar.Inventory
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler
    {
        [Header("组件获取")] [SerializeField] private Image slotImage;
        [SerializeField] private TMP_Text amountText;//这里教程是textmeshprougui
        public Image slotHighlight;
        [SerializeField] private TMP_Text buttonName;
        [SerializeField] private Button button;
        [Header("格子类型")] public SlotType slotType;
        public bool isSelected;
        
        
        //物品信息
        public ItemDetails ItemDetails;
        public int itemAmount;
        public int slotIndex;

        //用于调用高亮的方法
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
        
        //激活的itemID
        public static int selectedID;
        
    
        private void Start()
        {
            isSelected = false;
            if (ItemDetails.itemID==0)
            {
                UpdateEmptySlot();
            }
        }



        /// <summary>
    /// 设置格子为空
    /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
            }

            ItemDetails.itemID = 0;
            itemAmount = 0;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }
    
    /// <summary>
    /// 更新格子的UI和信息
    /// </summary>
    /// <param name="item">ItemDetails</param>
    /// <param name="amount">持有数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            slotImage.enabled = true;
            ItemDetails = item;
            slotImage.sprite = ItemDetails.itemIcon;
            if (amount==0)
            {
   
                return;
            }
            itemAmount = amount;
            amountText.text = itemAmount.ToString();
            button.interactable = true;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0)
            {
                selectedID = 0;
                return;
            }
            isSelected = !isSelected;
            selectedID = ItemDetails.itemID;
            Debug.Log("现在点击的Slot是"+ItemDetails.name+"数量为"+itemAmount);
            inventoryUI.UpdateSlotHighLight(slotIndex);
            initActivateButton(ItemDetails);
        }
        /// <summary>
        /// 初始化装备按钮的状态，防止文字没有更新
        /// </summary>
        void initActivateButton(ItemDetails item)
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

