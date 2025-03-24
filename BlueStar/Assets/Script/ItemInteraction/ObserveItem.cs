using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ObserveItem : MonoBehaviour
{
 
    public Camera MainCamera;
    
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private Vector3 cameraPosition;
    private Vector3 foucusPosition;
    public Dictionary<GameObject, (Transform,Transform)> itemAnimation;
    public bool isObserving=false;
    public float rotationSpeed=1f;
    public  bool isChildItem=false;
    private GameObject childItem;
    public float cameraDistance = 3f;
    public GameObject InfomationUI;
    private int timer=0;
    public int dialougueID=0;
    public DialogueManager dialogManager;
    private GameObject UI_Front;
    public static bool canRotate = true;
    [Header("物品信息")] public string[] infos;
    public string nameString;
    public static int index = 0;
    private GameObject suggestUI;
    public static GameObject suggestUIInst;
    public static TMP_Text name;
    public static TMP_Text description;
    public static GameObject upHeader;
    public static GameObject downHeader;
    private Vector3 upStartPos;
    private Vector3 downStartPos;
    public static string[] infoStore;//存储的信息
    private int timer2 = 0;
    public static GameObject activeGameobject;
    

    private void Awake()
    {
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation.eulerAngles;
        MainCamera = GameObject.Find("------Camera------/MainCamera").GetComponent<Camera>();
        dialogManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        UI_Front=GameObject.Find("------UI------/UI_2D/Front");
        suggestUI= Resources.Load<GameObject>("Prefabs/UI/SuggestUIObserve");
        upHeader=GameObject.Find("------UI------/UI_2D").gameObject.transform.Find("UpHeader").gameObject;
        downHeader=GameObject.Find("------UI------/UI_2D").gameObject.transform.Find("DownHeader").gameObject;
        Debug.Log(upHeader.transform.position.ToString());
        Debug.Log(downHeader.transform.position.ToString());
        

        
        


    }

    private void OnEnable()
    {
        EventHandler.PassObserveInfos += getInfo;
        upStartPos = new Vector3(960, 1080, 0);
        downStartPos = new Vector3(960, -1080, 0);
    }

    private void OnDisable()
    {
        EventHandler.PassObserveInfos -= getInfo;
    }

    private void Update()
    {
        if (Camera.main!=null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从自定义相机发射射线
        RaycastHit hit;
        

        if (Physics.Raycast(ray, out hit,20f,LayerMask.GetMask("Interactive")))
        {

            // 如果射线击中物体，执行放大操作
            if (hit.transform == transform)
            {
                Debug.Log("射线击中了物体");
                if (Input.GetMouseButtonDown(0) && suggestUIInst==null )
                {
                    if (isObserving==false)
                    {
                        EventHandler.CallPassObserveInfos(infos);
                        activeGameobject = this.gameObject;
                        suggestUIInst= Instantiate(suggestUI, GameObject.Find("------UI------/UI_2D").gameObject.transform);
                        name=suggestUIInst.gameObject.transform.Find("Name").GetComponent<TMP_Text>();
                        name.text = nameString;
                        description=suggestUIInst.gameObject.transform.Find("Description").GetComponent<TMP_Text>();
                        description.text = infoStore[0];
                        cameraPosition = MainCamera.transform.position;
                        foucusPosition = cameraPosition + MainCamera.transform.forward * cameraDistance;
                        //transform.position = foucusPosition;
                        // ObserveItem.upHeader.transform.DOMove(new Vector3(960, 880, 0) , 1);
                        // ObserveItem.downHeader.transform.DOMove(new Vector3(960, -880, 0) , 1);
                        EventHandler.CallMoveHeader(true);
                        transform.DOMove(foucusPosition, 1f);
                        timer += 1;
                    }
                    if (InfomationUI != null)
                    {
                        InfomationUI.SetActive(true);
                        if (InfomationUI.activeSelf==true)
                        {
                            timer = 0;
                        }
                        // if (isChildItem && timer >= 2)
                        // {
                        //
                        //     InfomationUI.SetActive(true);
                        //
                        // }
                        if (timer >= 2)
                        {

                            InfomationUI.SetActive(true);
                     
                        }
                        
                    }
                    

                    if (dialougueID !=0)
                    {
                        DialogueManager.currentDialogueBeginID = dialougueID;
                        //dialogManager.Awake();
                        //dialogManager.Start();
                        //UI_Front.SetActive(true);
                        dialougueID = 0;
                    }
                    
                }

            }
        }
        



        }

        if (isObserving && isChildItem==false )
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            transform.Rotate(v * rotationSpeed,0,h*rotationSpeed);
            
        }


        if (Input.GetMouseButtonDown(1))
        {
            transform.DOMove(originalPosition, 1f);
            transform.DORotate(originalRotation, 1);
            EventHandler.CallResetHeader(true);
            Destroy(ObserveItem.suggestUIInst.gameObject);
            isObserving = false;
            if (InfomationUI!= null)
            {
                InfomationUI.SetActive(false);
            }
            canRotate = true;
        }
        
        if (isChildItem && isObserving)
        {
            canRotate = false;  // 禁用父子物体的旋转功能
        }


        
    }

    void getInfo(string[] infos)
    {
        infoStore = infos;
    }
    void onUpdateInformations()
    {

        if (ObserveItem.index < ObserveItem.infoStore.Length-1)
        {
            ObserveItem.index += 1;
            ObserveItem.description.text = ObserveItem.infoStore[index];

        }
        else if (ObserveItem.index >= ObserveItem.infoStore.Length - 1)
        {
            EventHandler.CallResetHeader(true);
            Destroy(ObserveItem.suggestUIInst.gameObject);
            ObserveItem.index = 0;
            activeGameobject.GetComponent<ObserveItem>().isObserving=true;
        }
    }

    public void OnOptionClick()
    {
        onUpdateInformations();
    }
    public void UpdateHeader()
    {
    
        // 恢复到初始位置
        upHeader.transform.DOMove(upStartPos, 1);
        downHeader.transform.DOMove(downStartPos, 1);
       
    }
    

}

