using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlueStar.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private GameObject UI;
        private GameObject itemPickUpUI;
        private GameObject itemPickUpUIInst;
        private static bool isPickedUp = false;
        private static bool isDestroy = false;//用于不选择捡起时的退出
        private GameObject UI_Front;
        private Item item;
        private AudioSource audioFetch;
        public AudioSource audioPicked;
        private TMP_Text header;
        private TMP_Text description;
        private Image icon;
        

        private void Awake()
        {
            UI = GameObject.Find("------UI------/UI_2D");
            UI_Front = GameObject.Find("------UI------/UI_2D/Front");
            itemPickUpUI = Resources.Load<GameObject>("Prefabs/UI/ItemPickUpWidget");
            
        }

        private void Update()
        {
            ClickItem();
            if (item != null)
            {
                item._itemDetails.canPickedup = isPickedUp;
                if(item._itemDetails.canPickedup)
                {
    
                    Destroy(itemPickUpUIInst.gameObject);
                    InventoryManager.Instance.AddItem(item,true);
                    //UI_Front.SetActive(true);
                    //UI.GetComponent<CanvasGroup>().alpha = 0;
                    //UI.GetComponent<CanvasGroup>().interactable = false;
                    isPickedUp = false;

                }

                if (isDestroy)
                {
                    Destroy(itemPickUpUIInst.gameObject);
                    //UI_Front.SetActive(true);
                    //UI.GetComponent<CanvasGroup>().alpha = 0;
                    //UI.GetComponent<CanvasGroup>().interactable = false;
                    isDestroy = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("碰到了场景物体");
            item = other.GetComponent<Item>();

            if (item != null && item.interactionMode == InteractionMode.Trigger)
            {
                UI_Front.SetActive(false);
                itemPickUpUIInst=Instantiate(itemPickUpUI, GameObject.Find("------UI------/UI_2D").gameObject.transform);
                header = itemPickUpUIInst.transform.Find("Name").transform.Find("NameText").GetComponent<TMP_Text>();
                description = itemPickUpUIInst.transform.Find("Detail").transform.Find("DetailText").GetComponent<TMP_Text>();
                icon = itemPickUpUIInst.transform.Find("Icon").transform.Find("IconImage").GetComponent<Image>();
                header.text = item._itemDetails.name;
                description.text = item._itemDetails.itemDescriptions;
                icon.sprite = item._itemDetails.itemIcon;
                audioFetch = itemPickUpUIInst.GetComponent<AudioSource>();
                audioFetch.Play();
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            item = other.GetComponent<Item>();
            if (item != null && itemPickUpUIInst != null && item.interactionMode == InteractionMode.Trigger)
            {
                Destroy(itemPickUpUIInst.gameObject);
            }
        }

        private void ClickItem()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从自定义相机发射射线
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200f, LayerMask.GetMask("Item")))
                {
                    item=hit.collider.GetComponent<Item>();
                    UI_Front.SetActive(false);
                    itemPickUpUIInst=Instantiate(itemPickUpUI, GameObject.Find("------UI------/UI_2D").gameObject.transform);
                    header = itemPickUpUIInst.transform.Find("Name").transform.Find("NameText").GetComponent<TMP_Text>();
                    description = itemPickUpUIInst.transform.Find("Detail").transform.Find("DetailText").GetComponent<TMP_Text>();
                    icon = itemPickUpUIInst.transform.Find("Icon").transform.Find("IconImage").GetComponent<Image>();
                    header.text = item._itemDetails.name;
                    description.text = item._itemDetails.itemDescriptions;
                    icon.sprite = item._itemDetails.itemIcon;
                    audioFetch = itemPickUpUIInst.GetComponent<AudioSource>();
                    audioFetch.Play();
                }
            }
        }
        
        //按钮的方法
/// <summary>
/// 
/// </summary>
        public void ClickPickUpBotton()
        {
    
            isPickedUp = true;
        }
        
        public void ClickNotPickUpBotton()
        {
            isPickedUp = false;
            isDestroy = true;

        }
    }

}
