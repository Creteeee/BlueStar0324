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
        [SerializeField] private CanvasGroup carryButtonCanvasGroup;
        [SerializeField] private CanvasGroup dropButtonCanvasGroup;


        private void Awake()
        {
            initialImage = Icon.sprite;
            carryButtonCanvasGroup.alpha = 0;
            carryButtonCanvasGroup.interactable = false;
            dropButtonCanvasGroup.alpha = 0;
            dropButtonCanvasGroup.interactable = false;
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
                    switch (item.itemType)
                    {
                        case ItemType.bulletFreeze:
                            carryButtonCanvasGroup.alpha = 0;
                            carryButtonCanvasGroup.interactable = false;
                            dropButtonCanvasGroup.alpha = 1;
                            dropButtonCanvasGroup.interactable = true;
                            return;
                        case ItemType.drug:
                            carryButtonCanvasGroup.alpha = 1;
                            carryButtonCanvasGroup.interactable = true;
                            dropButtonCanvasGroup.alpha = 1;
                            dropButtonCanvasGroup.interactable = true;
                            return;
                        case ItemType.tool:
                            carryButtonCanvasGroup.alpha = 1;
                            carryButtonCanvasGroup.interactable = true;
                            dropButtonCanvasGroup.alpha = 0;
                            dropButtonCanvasGroup.interactable = false;
                            return;
                        case ItemType.card:
                            carryButtonCanvasGroup.alpha = 1;
                            carryButtonCanvasGroup.interactable = true;
                            dropButtonCanvasGroup.alpha = 0;
                            dropButtonCanvasGroup.interactable = false;
                            return;
                        case ItemType.weapon:
                            carryButtonCanvasGroup.alpha = 1;
                            carryButtonCanvasGroup.interactable = true;
                            dropButtonCanvasGroup.alpha = 0;
                            dropButtonCanvasGroup.interactable = false;
                            return;
                            
                    }
                    return;
                    
                }
                ResetSelectedUI();


            }

            if (SlotUI.selectedID==0)
            {
                ResetSelectedUI();
                carryButtonCanvasGroup.alpha = 0;
                carryButtonCanvasGroup.interactable = false;
                dropButtonCanvasGroup.alpha = 0;
                dropButtonCanvasGroup.interactable = false;
                return;
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

