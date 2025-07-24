using System.Collections;
using BlueStar.Inventory;
using DG.Tweening;
using UnityEngine;

public class DetectPlayerEnter : MonoBehaviour
{
    [SerializeField]private bool isEntered = false;
    public bool isFocusing = false;
    public GameObject interactUIWidget;
    public Transform foucusPoint;
    public int dialougueID = 0;
    public DialogueManager dialogManager;
    private GameObject player;
    private LayerMask playerLayer;
    public Camera MainCamera;
    private Vector3 initialCameraPosition;  // 记录相机初始位置
    private Quaternion initialCameraRotation;  // 记录相机初始旋转
    private TerraCamera _terraCamera;
    private Vector3 pos;
    private Vector3 scale;
    private CanvasGroup blackBG;
    public static GameObject currentInteractObj;
    public static GameObject FocusCancelButton;
    

    

    private void Awake()
    {
        MainCamera = GameObject.Find("------Camera------/MainCamera").GetComponent<Camera>();
        player = GameObject.Find("Terra");
        _terraCamera = MainCamera.GetComponent<TerraCamera>();
        if (interactUIWidget!=null)
        {
            interactUIWidget.SetActive(false);
        }
        //scale = player.transform.localScale;
        blackBG = GameObject.Find("------UI------/UI_2D/BlackBG").gameObject.GetComponent<CanvasGroup>();
        currentInteractObj = null;
        //挂载场景中物体，注意有没有改名字
        FocusCancelButton=GameObject.Find("------UI------/UI_2D").gameObject.transform.Find("FocusCancelButton").gameObject;


    }

    private void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        FocusCancelButton.SetActive(false);
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

                }

                player.GetComponent<Controller_Terra>().enabled = false;
                player.GetComponent<Animator>().enabled = false;
                Controller_Terra.audio.Stop();
                
            }
        }
        

        // if (isFocusing )  // 按下 Escape 键时
        // {
        //   
        //     if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        //     {
        //         Debug.Log("开始重置摄像头");
        //         StartCoroutine(ResetCameraPosition());
        //
        //         if (ItemPickUp.itemPickUpUIInst!=null)
        //         {
        //             Destroy(ItemPickUp.itemPickUpUIInst.gameObject);
        //             ItemPickUp.itemPickUpUIInst = null;
        //         }
        //
        //     }
        //
        // }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUIWidget.SetActive(true);
            isEntered = true;
            player = other.gameObject;
            currentInteractObj = this.gameObject;

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

    public IEnumerator ChangeCameraPosition()
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
        player.GetComponent<Controller_Terra>().enabled = false;
        MainCamera.cullingMask &= ~(1 <<playerLayer); 
        blackBG.DOFade(0, 0.5f);
        isFocusing = true;
        isEntered = false;
        InventoryManager.Instance.suggestGlobal.SetActive(false);
        FocusCancelButton.SetActive(true);
    }

    public IEnumerator ResetCameraPosition()
    {
        blackBG.DOFade(1, 0.5f);
        yield return new WaitForSeconds(1f);
        MainCamera.gameObject.transform.position = initialCameraPosition;
        MainCamera.gameObject.transform.rotation = initialCameraRotation;
        player.GetComponent<Controller_Terra>().enabled = true;
        player.GetComponent<Animator>().enabled = true;
        MainCamera.cullingMask |= 1<< playerLayer;
        _terraCamera.enabled = true;
        blackBG.DOFade(0, 0.5f);
        isFocusing = false;
        isEntered = true;
        InventoryManager.Instance.suggestGlobal.SetActive(true);
        FocusCancelButton.SetActive(false);
    }

    public void Reset()
    {
        //interactUIWidget.SetActive(false);
        //isEntered = false;
        StartCoroutine(ResetCameraPosition());
        currentInteractObj.GetComponent<DetectPlayerEnter>().enabled = false;
    }

    public static void CancelFocusing()
    {
        var detect = currentInteractObj.GetComponent<DetectPlayerEnter>();
        if ( detect.isFocusing )  // 按下 Escape 键时
        {
            Debug.Log("开始重置摄像头");
            detect.StartCoroutine(detect.ResetCameraPosition());

            if (ItemPickUp.itemPickUpUIInst!=null)
            {
                Destroy(ItemPickUp.itemPickUpUIInst.gameObject);
                ItemPickUp.itemPickUpUIInst = null;
            }
        }
    }
}
