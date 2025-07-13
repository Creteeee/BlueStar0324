using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

namespace BlueStar.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("玩家背包UI")] [SerializeField] private GameObject bagUI;
        [Header("操作提示UI")] [SerializeField] private GameObject operationSuggestUI;
        private bool bagOpened;
        [Header("地图")] [SerializeField] private GameObject mapUI;

        [Header("UI动画相机和人物的状态机")] 
        public GameObject UIAnimCamera;
        public Animator _PlayerAnimator;
        public Animator _UIAnimCameraAnimator;

        [Header("玩家的健康值和子弹值")]//这里写了UI有点乱但是就这样吧
        public Slider HealthBar;
        public TMP_Text BulletCountTex;
        public int BulletCount;
        public Material recoverMat;
        
        
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;

        [Header("背包数据")] public InventoryBag_SO playerBag;
        [Header("2DUI的Canvas")] [SerializeField]
        private GameObject canvas;

        public GameObject suggestGlobal;

        private GameObject carryWeaponUI;
        


        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            bagOpened = bagUI.activeInHierarchy;
            BulletCountTex.text = BulletCount.ToString();
            carryWeaponUI = Resources.Load<GameObject>("Prefabs/UI/UI_CarryWeapon");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OpenBagUI();
                //canvas.GetComponent<CanvasGroup>().alpha =bagOpened?1:0;
                //canvas.GetComponent<CanvasGroup>().interactable = bagOpened?true:false;
                if (!bagOpened)
                {
                    bagUI.GetComponent<CanvasGroup>().alpha = 1;
                    bagUI.GetComponent<CanvasGroup>().interactable = true;
                    operationSuggestUI.GetComponent<CanvasGroup>().alpha = 0;
                    operationSuggestUI.GetComponent<CanvasGroup>().interactable = false;
                    operationSuggestUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    _UIAnimCameraAnimator.SetFloat("Blend",0);
                    _PlayerAnimator.SetFloat("Blend",0);
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                mapUI.SetActive(!mapUI.activeSelf);
            }

            if (mapUI.activeSelf)
            {
                PostProcessingManager.Instance.pixelizeRenderPassFeature.settings.LowResHeight = 1920;
                PostProcessingManager.Instance.pixelizeRenderPassFeature.settings.LowResWidth = 1080;
            }
            else if (!mapUI.activeSelf)
            {
                PostProcessingManager.Instance.pixelizeRenderPassFeature.settings.LowResHeight = 405;
                PostProcessingManager.Instance.pixelizeRenderPassFeature.settings.LowResWidth = 720;
            }


        }

        public ItemDetails GetItemDetails(int ID)
        {
            var original = itemDataList_SO.ItemDetailsList.Find(i => i.itemID == ID);
            if (original != null)
            {
                return new ItemDetails()
                {
                    itemID = original.itemID,
                    name = original.name,
                    itemType = original.itemType,
                    itemIcon = original.itemIcon,
                    itemObject = original.itemObject,
                    itemDescriptions = original.itemDescriptions,
                    canPickedup = original.canPickedup,
                    canDropped = original.canDropped,
                    canCarried = original.canCarried
                };
            }
            else
            {
                return null;
            }
        }

        public void AddItem(Item item, bool toDestroy)
        {
            if (toDestroy)
            {
                //先判断背包是否有该物品
                var index = GetItemIndexBag(item.itemID);
                AddItemIndex(item.itemID,index,item.itemAmount);
                //再判断是否有空位
                InventoryItem newItem = new InventoryItem();
                newItem.itemID = item.itemID;
                newItem.itemAmount = item.itemAmount;
                Debug.Log("拾取了物体，ID："+GetItemDetails(item.itemID).itemID+"Name:"+GetItemDetails(item.itemID).name);
                Destroy(item.gameObject);

                if (item._itemDetails.itemType==ItemType.weapon)
                {
                    Instantiate(carryWeaponUI, GameObject.Find("------UI------/UI_2D").gameObject.transform);
                }
                
                //更新背包UI
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            }
        }

        /// <summary>
        /// 使用后减少数量
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isUse"></param>
        public void UseItem(int itemID, bool isUse)
        {
            if (isUse)
            {
                var index = GetItemIndexBag(itemID);
                int currentAmount = playerBag.itemList[index].itemAmount;
                if (currentAmount > 0)
                {
                    var itemNew = new InventoryItem() { itemID = itemID, itemAmount = currentAmount-1 };
                    playerBag.itemList[index] = itemNew;
                    
                    //如果使用过后的物品数量变为0，则删除物品
                    int refreshedAmount = playerBag.itemList[index].itemAmount;
                    if (refreshedAmount<=0)
                    {
                        DeleteItem(itemID);
                    }
                }
                //更新背包UI
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
                
            }
        }
        /// <summary>
        /// 删除背包SO中对应index的物体
        /// </summary>
        /// <param name="itemID"></param>

        private void DeleteItem(int itemID)
        {
            var index = GetItemIndexBag(itemID);
            var itemNew = new InventoryItem() { itemID = 0, itemAmount = 0 };
            playerBag.itemList[index] = itemNew;
            EventHandler.CallResetEmptySlot(index);
            if (itemID==1006)
            {
                BulletCount = 0;
                BulletCountTex.text = BulletCount.ToString();
            }
        }
        
        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagOpacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                {
                    return true;
                }
            }

            return false;
        }
        
/// <summary>
/// 在有相同物体时使用，返回背包中物体对应的列表位置index
/// </summary>
/// <param name="ID"></param>
/// <returns></returns>
        public int GetItemIndexBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                {
                    return i;
                }
            }

            return -1;//-1代表没有相同的物品
        }
/// <summary>
/// 添加背包物品
/// </summary>
/// <param name="ID"></param>
/// <param name="index"></param>
/// <param name="amount"></param>
        private void AddItemIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagOpacity())//背包没这个物体但有空位
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
                //还有种情况没有相同的也没空位就不加
            }
            else if(index !=-1 )
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem() { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }

        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
            operationSuggestUI.SetActive(bagOpened);
            UIAnimCamera.SetActive(bagOpened);
            PostProcessingManager.Instance.pixelizeRenderPassFeature.SetActive(!bagOpened);
            if (!bagOpened)
            {
                recoverMat.SetFloat("_Offset",-3f);
                _UIAnimCameraAnimator.SetFloat("Blend",0f);
                _PlayerAnimator.SetFloat("Blend",0f);
            }
            
        }

        public void ChangeToOperationSuggest()
        {
            bagUI.GetComponent<CanvasGroup>().alpha = 0;
            bagUI.GetComponent<CanvasGroup>().interactable = false;
            operationSuggestUI.GetComponent<CanvasGroup>().alpha = 1;
            operationSuggestUI.GetComponent<CanvasGroup>().interactable = true;
            operationSuggestUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
            StartCoroutine(FadeBlendValue(0, 1, 1));
        }

        public void ChangeToBag()
        {
            bagUI.GetComponent<CanvasGroup>().alpha = 1;
            bagUI.GetComponent<CanvasGroup>().interactable = true;
            operationSuggestUI.GetComponent<CanvasGroup>().alpha = 0;
            operationSuggestUI.GetComponent<CanvasGroup>().interactable = false;
            operationSuggestUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
            StartCoroutine(FadeBlendValue(1, 0, 1));
        }
        
        public IEnumerator FadeBlendValue(float start, float end, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float currentValue = Mathf.Lerp(start, end, t);
                
                _UIAnimCameraAnimator.SetFloat("Blend",currentValue);
                _PlayerAnimator.SetFloat("Blend",currentValue);
            
                yield return null;
            }
            
            _UIAnimCameraAnimator.SetFloat("Blend",end);
            _PlayerAnimator.SetFloat("Blend",end);

        }

        public void UseBullet()
        {
            int bulletItemID;
            for (int i = playerBag.itemList.Count - 1; i >= 0; i--)
            {
                if (playerBag.itemList[i].itemID==1006)
                {
                    bulletItemID = playerBag.itemList[i].itemID;
                    UseItem(bulletItemID,true);
                }
            }

            
            BulletCountTex.text = BulletCount.ToString();
        }

        public IEnumerator HealthRecoverVFX(float start, float end, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float currentValue = Mathf.Lerp(start, end, t);
                recoverMat.SetFloat("_Offset",currentValue);
                yield return null;
            }
            recoverMat.SetFloat("_Offset",end);
        }
        
        private void OnEnable()
        {
            EventHandler.DestroyObject += onDestroyDontDestroyOnLoadObjects;
        }

        private void OnDisable()
        {
            EventHandler.DestroyObject -= onDestroyDontDestroyOnLoadObjects;
        }

        void onDestroyDontDestroyOnLoadObjects(bool isDestroy)
        {
            Destroy(this.gameObject);
        }

    }
    
    
    
}

