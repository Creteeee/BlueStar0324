using System;
using System.Collections;
using System.Collections.Generic;
using BlueStar.Inventory;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class DetectPlayerItem : MonoBehaviour
{
    [Header("交互类型")] [SerializeField]private InteractionMode mode;
    [Header("物品信息")] [SerializeField]private int expectID;
    private GameObject suggestUI;
    private GameObject suggestUIInst;
    private GameObject suggestE;
    private GameObject suggestEInst;
    private Transform instTransform;
    private Item item;
    private bool isIneracted=false;
    public bool isFinished = false;
    static public GameObject upHeader;
    static public GameObject downHeader;
    private Vector3 upStartPos;
    private Vector3 downStartPos;
    [Header("关于这个交互装置的介绍")][SerializeField] private string[] Informations;
    private int infoIndex=0;

    [Header("这个装置的TimeLine")] [SerializeField]
    private PlayableDirector _director;

    public static GameObject currentObj;

    private void Awake()
    {
        Vector3 posNew = new Vector3(0, 5, -2);
        // instTransform = this.gameObject.transform;
        // instTransform.position = this.gameObject.transform.position + posNew;
        suggestE = Resources.Load<GameObject>("Prefabs/UI/SuggestESign");
        suggestUI = Resources.Load<GameObject>("Prefabs/UI/SuggestUI");
        //upHeader=GameObject.Find("------UI------/UI_2D").gameObject.transform.Find("UpHeader").gameObject;
        //downHeader=GameObject.Find("------UI------/UI_2D").gameObject.transform.Find("DownHeader").gameObject;
        _director = this.GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
      
        
    }

    private void OnDisable()
    {
        
    }
    void Start()
    {
        Debug.Log("已绑定");
        // 监听 Timeline 结束事件
        _director.stopped += OnTimelineStopped;
    }
    void OnDestroy()
    {
        // 取消事件绑定
        _director.stopped -= OnTimelineStopped;
    }

    private void Update()
    {
        if (mode == InteractionMode.Click)
        {
           Click();
        }
        

        if (Input.GetKeyDown(KeyCode.E)&& mode == InteractionMode.Trigger)
        {
            EventHandler.CallShowExpectedItemUI(isIneracted,expectID,Informations);
        }
        

  
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("玩家进入门中");
        if (other.CompareTag("Player"))
        {
            isIneracted = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isIneracted = false;
            
        }
        
    }

    private void Click()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit,20f))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform==transform)
                {
                    Debug.Log("CCC");
                    isIneracted = true;
                    EventHandler.CallShowExpectedItemUI(isIneracted,expectID,Informations);
                    currentObj = hit.transform.gameObject;
                }
            }
        }
    }

    public void onPlayTimeLine(bool isPlay)
    {
        if (_director!=null)
        {
            
            // upStartPos = upHeader.transform.position;
            // downStartPos = downHeader.transform.position;
            //
            // upHeader.transform.DOMove(upStartPos + new Vector3(0, -200, 0), 1);
            // downHeader.transform.DOMove(downStartPos + new Vector3(0, +200, 0), 1);
            EventHandler.CallMoveHeader(true);

            _director.Play();
        }
    }

    public void UpdateHeader()
    {
        Debug.Log("invoke调用了");
        // 恢复到初始位置
        EventHandler.CallResetHeader(true);
        if (currentObj != null)
        {
            currentObj.gameObject.GetComponent<DetectPlayerItem>().enabled = false;
        }
        
    }
    
    void OnTimelineStopped(PlayableDirector director)
    {
        Debug.Log("timeline播完");
        // Timeline 播放完后，恢复位置
        UpdateHeader();
    }
    
    
}
