using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueStar.Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;
        public int itemAmount;
        private GameObject _gameObject;
        public ItemDetails _itemDetails;
        public InteractionMode interactionMode=InteractionMode.Trigger;

        private void Start()
        {
            if (itemID != 0)
            {
               Init(itemID); 
            }

            _itemDetails.canPickedup = false;

        }

        public void Init(int ID)
        {
            itemID = ID;
            _itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            //itemAmount = _itemDetails.itemAmount;//获取拾取物体的个数，子弹通常有好几个
            if (_itemDetails != null)
            {
                _gameObject = _itemDetails.itemObject;
                Instantiate(_gameObject, this.transform);
            }
        }
    }
}

