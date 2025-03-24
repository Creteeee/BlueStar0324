using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  
    
namespace BlueStar.Inventory
{
    public class SelectedUI : MonoBehaviour
    {
        [Header("组件信息")] 
        [SerializeField] private TMP_Text Name;
        [SerializeField] private TMP_Text Description;
        [SerializeField] private Image Icon;
        private Sprite initialImage;
        public int ID;
        private ItemDetails item;


        private void Awake()
        {
            initialImage = Icon.sprite;
        }

        private void Update()
        {
            if (SlotUI.selectedID != 0)
            {
                item = InventoryManager.Instance.GetItemDetails(SlotUI.selectedID);
                if (item !=null)
                {
                    Name.text = item.name;
                    Description.text = item.itemDescriptions;
                    Icon.sprite = item.itemIcon;
                    return;
                    
                }
                ResetSelectedUI();
            }

            if (SlotUI.selectedID==0)
            {
                ResetSelectedUI();
            }

        }
        

        private void ResetSelectedUI()
        {
            Name.text = "";
            Description.text = "";
            Icon.sprite = initialImage;


        }
        

    }
}

