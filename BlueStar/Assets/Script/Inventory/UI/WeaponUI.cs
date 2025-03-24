using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BlueStar.Inventory
{
    public class WeaponUI : MonoBehaviour
    {
        [Header("UI组件")] [SerializeField] private Image icon;
        [SerializeField] private TMP_Text name;
        private Sprite initialIcon;

        private void Awake()
        {
            initialIcon = icon.sprite;
        }

        private void Update()
        {
            if (ActivateButtonUI.WeaponID != 0)
            {
                ItemDetails item = InventoryManager.Instance.GetItemDetails(ActivateButtonUI.WeaponID);
                icon.sprite = item.itemIcon;
                name.text = item.name;
            }
            else
            {
                icon.sprite = initialIcon;
            }
        }
    }
}

