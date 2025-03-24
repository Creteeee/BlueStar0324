using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;

public class DetectPlayerEnter : MonoBehaviour
{
    private bool isEntered = false;
    public GameObject interactUIWidget;
    public Transform foucusPoint;
    public int dialougueID = 0;
    public DialogueManager dialogManager;
    private GameObject player;
    public Camera MainCamera;
    private Vector3 initialCameraPosition;  // 记录相机初始位置
    private Quaternion initialCameraRotation;  // 记录相机初始旋转
    private TerraCamera _terraCamera;
    private Vector3 pos;
    private Vector3 scale;
    private CanvasGroup blackBG;
    

    

    private void Awake()
    {
        MainCamera = GameObject.Find("------Camera------/MainCamera").GetComponent<Camera>();
        player = GameObject.Find("Terra");
        _terraCamera = MainCamera.GetComponent<TerraCamera>();
        interactUIWidget.SetActive(false);
        scale = player.transform.localScale;
        blackBG = GameObject.Find("------UI------/UI_2D/BlackBG").gameObject.GetComponent<CanvasGroup>();
        //挂载场景中物体，注意有没有改名字


    }

    private void Update()
    {
        if (isEntered)
        {
            if (Input.GetKeyDown(KeyCode.E))  // 按下 E 键时
            {
                Debug.Log("我按下了E");
                StartCoroutine(ChangeCameraPosition());
                

                // 开始对话
                if (dialougueID != 0)
                {
                    DialogueManager.currentDialogueBeginID = dialougueID;
                    //Debug.Log("可以开启对话:" + gameObject.name);
                    //dialogManager.Awake();
                    //dialogManager.Start();

                }

                player.GetComponent<Controller_Terra>().enabled = false;
            }
        }
        
        //防止player挡住摄像机，所以暂时改变它的位置；但是这样会导致走出碰撞体，isentered会变为false；因此保持这个状态下isentered=true
        // if (player.transform.position !=pos && MainCamera.transform.position != initialCameraPosition )
        // {
        //     isEntered = true;
        // }
        
        if (Input.GetKeyDown(KeyCode.Escape) )  // 按下 Escape 键时
        {
           // player.transform.position = pos;
            if (isEntered)
            {
                StartCoroutine(ResetCameraPosition());

            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUIWidget.SetActive(true);
            isEntered = true;
            player = other.gameObject;
            //pos = player.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUIWidget.SetActive(false);
            isEntered = false;
        }
        isEntered = false;
    }

    private IEnumerator ChangeCameraPosition()
    {
        blackBG.DOFade(1, 0.5f);
        yield return new WaitForSeconds(1f);
        _terraCamera.enabled = false;
        // 记录当前相机的位置和旋转（在改变之前）
        initialCameraPosition = MainCamera.transform.position;
        initialCameraRotation = MainCamera.transform.rotation;
        // 改变相机的位置和旋转
        MainCamera.gameObject.transform.position = foucusPoint.position;
        MainCamera.gameObject.transform.rotation = foucusPoint.rotation;
        Vector3 posNew = new Vector3(MainCamera.transform.position.x, 0, MainCamera.transform.position.z-1);
        Vector3 scaleNew = new Vector3(0, 0.2f, 0f);
        //player.transform.position = posNew;
        player.transform.localScale = scaleNew;
        blackBG.DOFade(0, 0.5f);
    }

    private IEnumerator ResetCameraPosition()
    {
        blackBG.DOFade(1, 0.5f);
        yield return new WaitForSeconds(1f);
        player.transform.localScale = scale;
        // 恢复到记录的相机初始位置和旋转
        MainCamera.gameObject.transform.position = initialCameraPosition;
        MainCamera.gameObject.transform.rotation = initialCameraRotation;
        player.GetComponent<Controller_Terra>().enabled = true;
        _terraCamera.enabled = true;
        blackBG.DOFade(0, 0.5f);
    }
}
