using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace  BlueStar.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public SlotUI[] playerSlots;
        [SerializeField] private GameObject suggestUI;
        [SerializeField] private GameObject upHeader;
        [SerializeField] private GameObject downHeader;
        private Vector3 upHeaderPos;
        private Vector3 downHeaderPos;
  

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.ResetEmptySlot += onResetEmptySlot;
            EventHandler.MoveHeader += onMoveHeader;
            EventHandler.ResetHeader += onResetHeader;


        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.ResetEmptySlot -= onResetEmptySlot;
            EventHandler.MoveHeader -= onMoveHeader;
            EventHandler.ResetHeader -= onResetHeader;
       
        }

        private void Start()
        {
            //获取每个slot的index
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            upHeader=GameObject.Find("------UI------/UI_2D").gameObject.transform.Find("UpHeader").gameObject;
            downHeader=GameObject.Find("------UI------/UI_2D").gameObject.transform.Find("DownHeader").gameObject;

            upHeaderPos = upHeader.transform.position;
            downHeaderPos = downHeader.transform.position;

        }
        void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount>0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item,list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 更新高亮显示
        /// </summary>
        /// <param name="index"></param>

        public void UpdateSlotHighLight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHighlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHighlight.gameObject.SetActive(false);
                }
            }
        }
        
        void onResetEmptySlot(int index)
        {
            playerSlots[index].ItemDetails.itemID = 0;
            playerSlots[index].itemAmount = 0;
            
        }
        
        
        //在播放Timeline时加上黑框
        void onMoveHeader(bool isMove)
        {
            if (isMove)
            {
                upHeader.transform.DOMove(upHeaderPos + new Vector3(0, -200, 0), 1);
                downHeader.transform.DOMove(downHeaderPos + new Vector3(0, +200, 0), 1);
            }
        }

        void onResetHeader(bool isEnded)
        {
            if (isEnded)
            {
                upHeader.transform.DOMove(upHeaderPos, 1);
                downHeader.transform.DOMove(downHeaderPos, 1);
            }
        }




    }
}

