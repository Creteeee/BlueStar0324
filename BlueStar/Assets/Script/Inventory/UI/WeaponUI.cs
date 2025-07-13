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
        [Header("武器UI组件")] [SerializeField] private Image icon_Weapon;
        [SerializeField] private TMP_Text name_Weapon;
        private Sprite initialIcon_Weapon;

        [Header("道具UI组件")] [SerializeField] private TMP_Text name_Device;
        [SerializeField] private Image icon_Device;
        private Sprite initialIcon_Device;

        public static Image icon_Weapon_firsttime;
        public static TMP_Text name_Weapon_firsttime;

        private void Awake()
        {
            //icon_Weapon = GameObject.Find("------UI------/UI_2D/Status/Bag/IemEquipped/WeaponEquipped/Weapon/Icon").gameObject.GetComponent<Image>();
            //name_Weapon=  GameObject.Find("------UI------/UI_2D/Status/Bag/IemEquipped/WeaponEquipped/Weapon/Name").gameObject.GetComponent<TMP_Text>();
            initialIcon_Weapon =  icon_Weapon.sprite;
            initialIcon_Device = icon_Device.sprite;
            icon_Weapon_firsttime = icon_Weapon;
            name_Weapon_firsttime = name_Weapon;
        }

        private void OnEnable()
        {
            
            if (ActivateButtonUI.WeaponID != 0)
            {
                ItemDetails item = InventoryManager.Instance.GetItemDetails(ActivateButtonUI.WeaponID);
                icon_Weapon.sprite = item.itemIcon;
                name_Weapon.text = item.name;
            }
        }

        private void Update()
        {
            if (ActivateButtonUI.WeaponID != 0)
            {
                ItemDetails item = InventoryManager.Instance.GetItemDetails(ActivateButtonUI.WeaponID);
                icon_Weapon.sprite = item.itemIcon;
                name_Weapon.text = item.name;
            }
            else
            {
                name_Weapon.text = "";
                icon_Weapon.sprite = initialIcon_Weapon;
            }

            if (ActivateButtonUI.carriedID!=0)
            {
                ItemDetails item = InventoryManager.Instance.GetItemDetails(ActivateButtonUI.carriedID);
                icon_Device.sprite = item.itemIcon=item.itemIcon;
                name_Device.text = item.name;
            }
            else
            {
                name_Device.text = "";
                icon_Device.sprite = initialIcon_Device;
            }
            
            
        }


    }
}

