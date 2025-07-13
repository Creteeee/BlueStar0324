using System;
using System.Collections;
using System.Collections.Generic;
using BlueStar.Inventory;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;

public class ItemCarry : MonoBehaviour
{
    private int CarriedID;
    private int WeaponID;
    private int DeviceID;
    private GameObject suggestUI;
    private GameObject carryWeaponUI;
    public  static GameObject suggestUIInst;
    private bool IsInteractive=false;
    private bool lastInteractive;
    public static TMP_Text text_pre;
    public static TMP_Text name;
    private int expectedID;
    public static bool isCompare=true;//按钮返回是否进行背包比较的方法
    public static bool isClick = false;
    private bool isFinished;//存储完成状态
    private GameObject interactiveObject;
    public static GameObject nextButton;
    public static int index = 0;//交互物体描述的句子的当前序号
    public static string[] infos;
    public static GameObject chooseBar;
    
    

    private void Awake()
    {
        suggestUI = Resources.Load<GameObject>("Prefabs/UI/SuggestUI");
        carryWeaponUI = Resources.Load<GameObject>("Prefabs/UI/UI_CarryWeapon");
        
    }

    private void Update()
    {
        CarriedID = ActivateButtonUI.carriedID;
        if (isClick==true)
        {
            CompareID(text_pre, name);
            if (isCompare==false)
            {
                if (suggestUIInst!=null)
                {
                    Debug.Log("aaaa");
                    Destroy(suggestUIInst.gameObject);
                    suggestUIInst = null;

                }
            }

            isClick = false;
        }

        // if (DetectPlayerEnter.currentInteractObj!=null&&DetectPlayerEnter.currentInteractObj.GetComponent<Door_02>()==null )
        // {
        //     
        //     if (!DetectPlayerEnter.currentInteractObj.GetComponent<DetectPlayerEnter>().isFocusing)
        //     {
        //         Destroy(suggestUIInst);
        //         suggestUIInst = null;
        //     }
        // }

        
        

    }

    private void OnEnable()
    {
        EventHandler.ShowExpectItemUI += onInitiateSuggestUI;
    }

    private void OnDisable()
    {
        EventHandler.ShowExpectItemUI -= onInitiateSuggestUI;
    }

    
    private void onInitiateSuggestUI(bool isIneractive,int ID,string[] informations)
    {
        IsInteractive = isIneractive;
        if (IsInteractive && /*isIneractive !=lastInteractive &&*/suggestUIInst == null)
        {
            Debug.Log("正在生成提示UI"+isIneractive.ToString());
            suggestUIInst = Instantiate(suggestUI, GameObject.Find("------UI------/UI_2D").gameObject.transform);
            text_pre=suggestUIInst.gameObject.transform.Find("TextPre").GetComponent<TMP_Text>();
            name=suggestUIInst.gameObject.transform.Find("Name").GetComponent<TMP_Text>();
            nextButton = suggestUIInst.gameObject.transform.Find("NextButton").gameObject;
            chooseBar = suggestUIInst.gameObject.transform.Find("ChooseBar").gameObject;
            name.text = InventoryManager.Instance.GetItemDetails(ID).name;
            expectedID = ID; 
            infos = informations;
            index = 0;
            //当物品的介绍有内容时会触发下面的委托
            if (infos.Length !=0)
            {
                index = 0;
                text_pre.text = informations[index];
                name.gameObject.SetActive(false);
                chooseBar.SetActive(false);
            }
            else if (infos.Length==0)
            {
                text_pre.text = "需要";
                chooseBar.SetActive(true);
            }
            

        }
        // else if (IsInteractive==false && suggestUIInst !=null)
        // {
        //     Debug.Log("bbbb");
        //     Destroy(suggestUIInst.gameObject);
        //     suggestUIInst = null;
        // }
        

    }

    public void onUpdateInformation()
    {
        GameObject UI = ItemCarry.suggestUIInst;
        int Index = ItemCarry.index;
        String[] strs = ItemCarry.infos;
        Debug.Log("更新UI已被调用");
        if (UI!=null)
        {
            Debug.Log(UI.name+Index+strs);
            if (Index <strs.Length-1)
            {
                Index += 1;
                ItemCarry.index = Index;
                text_pre.text = strs[index];
             
            }
            else if(Index >= strs.Length-1)
            {
                Debug.Log("已经放完信息了");
                ItemCarry.text_pre.text = "需要";
                ItemCarry.name.gameObject.SetActive(true);
                ItemCarry.chooseBar.SetActive(true);
                ItemCarry.nextButton.SetActive(false);
                IsInteractive = false;
            }

        }
        else if (suggestUIInst==null)
        {
            Debug.Log("111");
        }
    }
    
    /// <summary>
    /// 比较手上拿的东西的ID是否和预期的ID一样
    /// </summary>
    void CompareID(TMP_Text text,TMP_Text name)
    {
        InventoryBag_SO bag = InventoryManager.Instance.playerBag;
        foreach (var item in bag.itemList)
        {
            if (item.itemID==expectedID)
            {
                //切换UI
                isCompare = false;
                isFinished = true;
                text.text = "已使用";
                name.text = InventoryManager.Instance.GetItemDetails(expectedID).name;
                if (suggestUIInst!=null)
                {
                    Destroy(suggestUIInst.gameObject,2);
                }
                //EventHandler.CallPlayTimeLine(isFinished);//通知DetectItem播放TimeLine
                if (interactiveObject!=null)
                {
                    interactiveObject.GetComponent<DetectPlayerItem>()?.onPlayTimeLine(true);
                    if (interactiveObject!=null)
                    {
                        interactiveObject.GetComponent<DetectPlayerItem>().isFinished = isFinished;
                        Debug.Log(interactiveObject.name+"isfinish:"+isFinished.ToString());
                    }
                }     
                //删除手上物品
                CarriedID = 0;
                //删除背包物品
                InventoryManager.Instance.UseItem(expectedID, true);
                //清空UI
                SlotUI.selectedID = 0;
                ActivateButtonUI.carriedID = 0;
            }
            else
            {
                text.text = "没有检测到道具";
                name.text = "";
                if (suggestUIInst!=null)
                {
                    Debug.Log("cccc");
                    Destroy(suggestUIInst.gameObject,2);
                }
            }
        }
        /*
        if (expectedID == CarriedID && CarriedID !=0 && isCompare == true )
        {
            //切换UI
            isCompare = false;
            isFinished = true;
            text.text = "已使用";
            name.text = InventoryManager.Instance.GetItemDetails(expectedID).name;
            if (suggestUIInst!=null)
            {
                Destroy(suggestUIInst.gameObject,2);
            }
            //EventHandler.CallPlayTimeLine(isFinished);//通知DetectItem播放TimeLine
            if (interactiveObject!=null)
            {
                interactiveObject.GetComponent<DetectPlayerItem>()?.onPlayTimeLine(true);
                if (interactiveObject!=null)
                {
                    interactiveObject.GetComponent<DetectPlayerItem>().isFinished = isFinished;
                    Debug.Log(interactiveObject.name+"isfinish:"+isFinished.ToString());
                }
            }     
            //删除手上物品
            CarriedID = 0;
            //删除背包物品
            InventoryManager.Instance.UseItem(expectedID, true);
            //清空UI
            SlotUI.selectedID = 0;
            ActivateButtonUI.carriedID = 0;
            //播放TimeLine如有
            
        }

        else if(expectedID!=CarriedID)
        {
            text.text = "没有检测到道具";
            name.text = "";
            if (suggestUIInst!=null)
            {
                Debug.Log("cccc");
                Destroy(suggestUIInst.gameObject,2);
            }
        }
        else
        {
            Debug.Log("有误");
        }
        */
    }

    public void EnableCompare()
    {
        isClick = true;
        isCompare = true;
    }
    public void DisableCompare()
    {
        isClick = true;
        isCompare = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactive"))
        {
            interactiveObject = other.gameObject;
            Debug.Log("交互物品的名称是"+interactiveObject.name);
            
        }
    }

    public void OnOptionClick( )
    {
        Debug.Log("我被Click了");
        onUpdateInformation();
    }
    
    
}
