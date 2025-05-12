using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BlueStar.Inventory;
using MeadowGames.UINodeConnect4;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;
using Slider = UnityEngine.UI.Slider;


public class UIManager_BattleMode : MonoBehaviour
{
    public List<GameObject> Emitterlist = new List<GameObject>();
    public GameObject emitter;

    public bool IsMaufactureUIActivated = false;
    public bool IsSpaceBattleUIActivated = true;

    private GameObject CanvasManufacture;
    private GameObject CanvasSpaceBattle;
    private GameObject CanvasWiki;

    private GameObject[] Celestial;
    List<MeshRenderer> CelestialRenderers=new List<MeshRenderer>();

    private GameObject Orbit_Camera;
    public bool isOrbitCameraActivated = false;
    public float speed = 0.1f;
    
    //先这么写 持续加速
    public GameObject leftButton;
    public GameObject rightButton;
    public Camera MainCamera;
    
    //Wiki
    public GameObject[] wikis ;
    public int wikiID = 0;
    
    //Temp 
    public GameObject FuelMenu;
    public GameObject FuelProgress;
    public TMP_Text textFuel;
    public TMP_Text ManuingEmiterName;
    
    //生产的条目
    [Header("生产相关UI")] 
    public GameObject ManuTab;
    public UICSystemManager uicManager;
    public GameObject[] ManuSlots;
    public static int ManuSlotIndex=-1;
    
    [SerializeField] private GameObject ManuSuggest;
    [SerializeField] private GameObject NotManuSuggest;
    private GameObject ManuSuggestInst;
    private GameObject NotManuSuggestInst;
    public static bool isManufacture;
    
    //预备的发射Slot
    public EmitterDataList_SO emitterDataList;
    public EmitterSlot[] emitterSlots;
    public bool isEmitterSlotsAvailable = true;
    public static int ActivatedSlotIndex = 0;
    
    //发射相关UI
    public GameObject LaunchButton;//发射的按钮，更新slot的UI时判断是否隐藏它
    public static TMP_Text EmitterName;
    public static Slider HealthBar;
    public static TMP_Text bulletLeftText;
    public bool isClickChangeEmitterButton = false;
    
    
    //预览的模型们
    public GameObject[] prelookModels;
    
    [Header("引导线")]
    //从发射器到鼠标的导引线
    private GameObject linePrefab;
    public static GameObject suggestLine;
    private LineRenderer suggestLineRenderer;
    public GameObject arrowIcon;
    public static GameObject arrowInst;
    public Camera orbitCamera;
    public static Vector3 bulletDirection;

    [Header("游戏进度")] public Slider killProgress;
    public TMP_Text killProgressText;
    public static int killCount=0;
    public GameObject finishGameUIInst;
    private int timer = 0;//防止一直调用携程
    public Transform TerraControlRoomTransform;

    
    
    
    
    private void OnEnable()
    {
        Debug.Log("UIManager_BattleMode 已启用");
        /*EventCenter_BattleMode.OnEmitterListUpdated += UpdateEmitterList;*/
        EventCenter_BattleMode.OnActivateManufactureUI += ActivateManufactureUI;
        EventCenter_BattleMode.OnActivateActivateSpaceBattleUI += ActivateSpaceBattleUI;
        EventCenter_BattleMode.OnUnlaunchedEmitterUpdated += InstantiateNewModel;

        // 缓存
        CanvasManufacture = GameObject.Find("Canvas_Manufacture");
        CanvasSpaceBattle = GameObject.Find("Canvas_SpaceBattle");
        CanvasWiki = GameObject.Find("Canvas_Wiki");
        Celestial = GameObject.FindGameObjectsWithTag("Celestial");
        foreach (GameObject child in Celestial)
        {
            CelestialRenderers.Add(child.GetComponent<MeshRenderer>());
        }

        foreach (MeshRenderer renderer in CelestialRenderers)
        {
            renderer.enabled = true;
        }

 
    }

    private void OnDisable()
    {
        /*EventCenter_BattleMode.OnEmitterListUpdated -= UpdateEmitterList;*/
        EventCenter_BattleMode.OnActivateManufactureUI -= ActivateManufactureUI;
        EventCenter_BattleMode.OnActivateActivateSpaceBattleUI -= ActivateSpaceBattleUI;
        EventCenter_BattleMode.OnUnlaunchedEmitterUpdated -= InstantiateNewModel;

    }

    private void Awake()
    {
        ManuSuggest = Resources.Load("Prefabs/UI/ManuSuggest").GameObject();
        NotManuSuggest = Resources.Load("Prefabs/UI/NotManuSuggest").GameObject();
        LaunchButton.SetActive(false);
        linePrefab = Resources.Load<GameObject>("Prefabs/Line/Line");
        suggestLine = Instantiate(linePrefab);
        suggestLineRenderer = suggestLine.GetComponent<LineRenderer>();
        suggestLine.SetActive(false);
        arrowInst = Instantiate(arrowIcon);
        arrowInst.SetActive(false);
    }

    private void Start()
    {
        Orbit_Camera = GameObject.FindWithTag("Orbit_Camera");
        CanvasManufacture.GetComponent<CanvasGroup>().alpha = 0;
        CanvasSpaceBattle.GetComponent<CanvasGroup>().alpha = 1;
        CanvasManufacture.GetComponent<CanvasGroup>().interactable = false;
        CanvasManufacture.GetComponent<CanvasGroup>().blocksRaycasts = false;

        CanvasSpaceBattle.GetComponent<CanvasGroup>().interactable = true;
        CanvasSpaceBattle.GetComponent<CanvasGroup>().blocksRaycasts = true;
        CanvasWiki.SetActive(false);
        FuelMenu.SetActive(false);
        FuelProgress.SetActive(false);
        
        OnMenuIndexChanged += (index) => ChangeManufactureItem(index);
        EmitterName=GameObject.Find("Canvas_SpaceBattle/EmitterName").GetComponent<TMP_Text>();
        EmitterName.text = "空槽位";
        HealthBar= GameObject.Find("Canvas_SpaceBattle/FuelLeft/Slider").GetComponent<Slider>();
        HealthBar.gameObject.SetActive(false);
        ManuTab.SetActive(true);
        bulletLeftText= GameObject.Find("Canvas_SpaceBattle/BulletLeft/Text").GetComponent<TMP_Text>();
        bulletLeftText.text="";
        killProgress.value = 0;

        //MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //注册按钮方法，告诉比较哪个生产链
    }

    //----------------------Emitter发射----------------------------------
    /*private void UpdateLaunchedEmitterID(int ID)
    {
        Emitterlist = emitterlist;
    }*/

    private void Update()
    {
  
        //在轨道模式下，激活对于飞船的操作
        if (isOrbitCameraActivated)
        {
            
            //发射了才能控制
            if (emitterSlots[ActivatedSlotIndex].isLaunched && !emitterSlots[ActivatedSlotIndex].isEmpty)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    emitterSlots[ActivatedSlotIndex].Emitter_Launched.GetComponent<Emitter>().acceleration += 0.05f;
                    Debug.Log( emitterSlots[ActivatedSlotIndex].emitterDetails.name+"的加速度是"+emitterSlots[ActivatedSlotIndex].Emitter_Launched.GetComponent<Emitter>().acceleration);
                }

                if (Input.GetKey(KeyCode.D))
                {
                    emitterSlots[ActivatedSlotIndex].Emitter_Launched.GetComponent<Emitter>().acceleration =
                        Mathf.Max(
                            emitterSlots[ActivatedSlotIndex].Emitter_Launched.GetComponent<Emitter>().acceleration -
                            0.05f, -4f);
                    //Debug.Log( emitterSlots[ActivatedSlotIndex].emitterDetails.name+"的加速度是"+emitterSlots[ActivatedSlotIndex].Emitter_Launched.GetComponent<Emitter>().acceleration);
                }

            }
            
        }
        
        //设置fuel的值
        if ( emitterSlots[ActivatedSlotIndex].isLaunched)
        {
            HealthBar.value = emitterSlots[ActivatedSlotIndex].fuel/emitterSlots[ActivatedSlotIndex].fuelTotal;
            DrawSuggestLine(emitterSlots[ActivatedSlotIndex].Emitter_Launched.transform.position,arrowInst,orbitCamera,suggestLineRenderer);

            if (isOrbitCameraActivated)
            {
                Ray ray = orbitCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray,out hit, 100f))
                {
                    if (!LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("AirWall")) 
                    {
                        emitterSlots[ActivatedSlotIndex].Shoot();
                    }
                    else
                    {
                        Debug.Log("打到了空气墙");
                    }
                }
                //emitterSlots[ActivatedSlotIndex].Shoot();
            }
            

        }
        //击杀进度

        killProgress.value = killCount / 5;
        killProgressText.text=killCount*20+"%";

        if (killCount==5&&timer==0)
        {
            Debug.Log("开始携程");
            StartCoroutine(FinishGame());
            timer += 1;
        }
        

    }

    public void LaunchEmitter()
    {
        // DataManager.updateLaunchedEmitterType=DataManager.emitterConfigs[DataManager.currentEmitterNumber].EmitterType;  
        // EventCenter_BattleMode.NotifyLaunchEmitter(); // 通知事件中心，更新
        // Debug.Log($"我是UI层，我发射了新的Emitter");
        emitterSlots[ActivatedSlotIndex].LaunchEmitter();
        LaunchButton.SetActive(false);
        suggestLine.SetActive(true);
        arrowInst.SetActive(true);
        
        
    }

    //-----------------------UI切换------------------------------
    public void ActivateManufactureUI(bool isMaufactureUIActivated)
    {
        IsMaufactureUIActivated = true; // 激活 Manufacture UI
        IsSpaceBattleUIActivated = false; // 禁用 SpaceBattle UI

        // 更新 Canvas 显示
        CanvasManufacture.GetComponent<CanvasGroup>().alpha = 1;
        CanvasSpaceBattle.GetComponent<CanvasGroup>().alpha = 0;

        // 确保交互状态也同步
        CanvasManufacture.GetComponent<CanvasGroup>().interactable = true;
        CanvasManufacture.GetComponent<CanvasGroup>().blocksRaycasts = true;

        CanvasSpaceBattle.GetComponent<CanvasGroup>().interactable = false;
        CanvasSpaceBattle.GetComponent<CanvasGroup>().blocksRaycasts = false;
        Orbit_Camera.SetActive(false);
        isOrbitCameraActivated = false;

    }

    public void ActivateSpaceBattleUI(bool isSpaceBattleUIActivated)
    {
        IsSpaceBattleUIActivated = true; // 激活 SpaceBattle UI
        IsMaufactureUIActivated = false; // 禁用 Manufacture UI

        // 更新 Canvas 显示
        CanvasManufacture.GetComponent<CanvasGroup>().alpha = 0;
        CanvasSpaceBattle.GetComponent<CanvasGroup>().alpha = 1;

        // 确保交互状态也同步
        CanvasManufacture.GetComponent<CanvasGroup>().interactable = false;
        CanvasManufacture.GetComponent<CanvasGroup>().blocksRaycasts = false;

        CanvasSpaceBattle.GetComponent<CanvasGroup>().interactable = true;
        CanvasSpaceBattle.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Orbit_Camera.SetActive(true);
        isOrbitCameraActivated = true;
    }
    
    //-----------------------实例化新的Model-----------------------

    private void InstantiateNewModel(int i)
    {
        Instantiate(DataManager.emitterConfigs[DataManager.updateUnlaunchedEmitterType].Model);
        //Instantiate(DataManager.emitterConfigs[1].Model);
        Debug.Log("我是UI层，我实例化了新的Model");
        DataManager.updateUnlaunchedEmitterType = 0;//清空未实例化的对象
    }
    
    //-----------------------给Emitter加速----------------------------
    public void onAcceleration()
    {
        DataManager.emitterAcceleration +=speed;
    }
    public void onDisAcceleration()
    {
        DataManager.emitterAcceleration -=speed;
    }

    public void EnableWikiBotton()
    {
        CanvasWiki.SetActive(true);
    }

    public void ExitWiki()
    {
        CanvasWiki.SetActive(false);
    }

    public void WikiNextPage()
    {
        for (int i = 0; i < wikis.Length; i++)
        {
            wikis[i].SetActive(false);
        }

        wikiID += 1;
        wikiID = Mathf.Clamp(wikiID, 0, wikis.Length );
        wikis[wikiID].SetActive(true);
    }
    public void WikiLastPage()
    {
        for (int i = 0; i < wikis.Length; i++)
        {
            wikis[i].SetActive(false);
        }

        wikiID -= 1;
        wikiID = Mathf.Clamp(wikiID, 0, wikis.Length );
        wikis[wikiID].SetActive(true);
    }

    public void EnableFuelMenu()
    {
        FuelMenu.SetActive(true);
    }

    public void EnableFuelProgress()
    {
        ManuTab.SetActive(false);
        FuelMenu.SetActive(false);
        FuelProgress.SetActive(true);
        ManuingEmiterName.text = GetEmitterDetails(ManuSlotIndex).name;
        StartCoroutine(UpdatePercentage());

    }
    private IEnumerator UpdatePercentage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 100)
        {
            elapsedTime += Time.deltaTime*10;
            float percentage = Mathf.Clamp01(elapsedTime / 100) * 100f;
            textFuel.text = $"{Mathf.RoundToInt(percentage)}%";
            yield return null;
        }

        // 确保最终值是 100%
        textFuel.text = "100%";
        //通知刷新EmitterSlot，这里将SO和UI的Index设成一样的了
        AddEmitter(ManuSlotIndex);
        FuelProgress.SetActive(false);
        uicManager.EndAnimation(ManuSlotIndex);
        ManuTab.SetActive(true);
        
    }

    public System.Action<int> OnMenuIndexChanged;
    
    public void EnableManufactureItem(GameObject MenufactureMenu)
    {
        
        MenufactureMenu.SetActive(true);
        ManuSlotIndex = 0;
        OnMenuIndexChanged?.Invoke(ManuSlotIndex);
        
    }

    public void ChangeManufactureItem(int index)
    {
        foreach (var slot in ManuSlots)
        {
            slot.SetActive(false);
        }
        ManuSlots[index].SetActive(true);
    }

    public void NextManufactureItem()
    {
        if (ManuSlotIndex<ManuSlots.Length-1)
        {
            ManuSlotIndex += 1;
        }
        else
        {
            ManuSlotIndex = 0;
        }
        OnMenuIndexChanged?.Invoke(ManuSlotIndex);
    }

    public void PreviousManufactureItem()
    {
        if (ManuSlotIndex>0)
        {
            ManuSlotIndex -= 1;
        }
        else
        {
            ManuSlotIndex = ManuSlots.Length - 1;
        }
        OnMenuIndexChanged?.Invoke(ManuSlotIndex);
    }

    public void CallMenufactureItem()
    {
        callCompareManufactureLine(ManuSlotIndex);
    }

    public void ExitMenufacture(GameObject menufactureMenu)
    {
        menufactureMenu.SetActive(false);
    }
    
    

    public void callCompareManufactureLine(int index)
    {
        checkEmitterSlot();
        UIManager_BattleMode.ManuSlotIndex = index;
        UICSystemManager.CompareSteps(index,isEmitterSlotsAvailable);
        showManufactureText(isManufacture);
    }

    public void showManufactureText(bool isCorrect)
    {
        if (isCorrect)
        {
            Debug.Log("链接正确，正在生产中");
            EnableFuelProgress();
            ManuSuggestInst=Instantiate(ManuSuggest, GameObject.Find("Canvas_Manufacture").gameObject.transform);
            ExitMenufacture(GameObject.Find("Canvas_Manufacture/ManufactureMenu"));
            Destroy(ManuSuggestInst,1.5f);
        }
        else
        {
            Debug.Log("链接错误");
            NotManuSuggestInst=Instantiate(NotManuSuggest, GameObject.Find("Canvas_Manufacture").gameObject.transform);
            Destroy(NotManuSuggestInst,1.5f);
        }
    }
/// <summary>
/// 检查是否有空位
/// </summary>
/// <param name="emitterID"></param>
    public void checkEmitterSlot()//前面调用方法的时候要注意一下
    {
        bool hasUpdated = false; // 用来记录有没有成功填进去

        foreach (EmitterSlot slot in emitterSlots)
        {
            if (slot.isEmpty)
            {
                hasUpdated = true;
                isEmitterSlotsAvailable = true;
                break;
            }
        }

        if (!hasUpdated)
        {
            // 所有slot都不是空的，这里执行备用操作
            isEmitterSlotsAvailable = false;

        }
    }

    public void AddEmitter(int EmitterID)
    {
        for (int i = 0; i < emitterSlots.Length; i++)
        {
            emitterSlots[i].defaultColorSlot();
            if (emitterSlots[i].isEmpty)
            {
                emitterSlots[i].UpadateSlot(GetEmitterDetails(EmitterID));
                emitterSlots[i].HighLightSlot();
                foreach (GameObject model in prelookModels)
                {
                    model.SetActive(false);
                }
                prelookModels[ManuSlotIndex].SetActive(true); // ManuSlotIndex指的是用的哪个船，i是哪个格子被激活
                ActivatedSlotIndex = i;
                LaunchButton.SetActive(true);
                ActivateSlot(i);// 这里有点重复回来改改
                break;
            }
        }
    }

    public void NextSlot()
    {
        InactiveSlot(ActivatedSlotIndex);
        if (ActivatedSlotIndex<emitterSlots.Length-1)
        {
            ActivatedSlotIndex += 1;
        }
        else
        {
            ActivatedSlotIndex = 0;
        }
        ActivateSlot(ActivatedSlotIndex);
    }

    public void PreviousSlot()
    {
        InactiveSlot(ActivatedSlotIndex);
        if (ActivatedSlotIndex>0)
        {
            ActivatedSlotIndex -= 1;
        }
        else
        {
            ActivatedSlotIndex = emitterSlots.Length - 1;
        }

        ActivateSlot(ActivatedSlotIndex);
    }

    public void ActivateSlot(int index)
    {
        foreach (EmitterSlot slot in emitterSlots)
        {
            slot.defaultColorSlot();
        }
        emitterSlots[index].HighLightSlot();
        foreach (GameObject model in prelookModels)
        {
            model.SetActive(false);
        }

        if (!emitterSlots[index].isEmpty)
        {
            EmitterName.text = emitterSlots[ActivatedSlotIndex].emitterDetails.name;
            //HealthBar.value = 1;
            HealthBar.value = emitterSlots[index].fuel / emitterSlots[index].fuelTotal;
            HealthBar.gameObject.SetActive(true); 
            prelookModels[emitterSlots[ActivatedSlotIndex].emitterDetails.ID].SetActive(true);
            bulletLeftText.text = emitterSlots[index].bullet.ToString() + "/" + emitterSlots[index].bulletTotal.ToString();
            
            if (!emitterSlots[index].isLaunched )
            {
                LaunchButton.SetActive(true);
                suggestLine.SetActive(false);
                arrowInst.SetActive(false);
            }
        }

        if (emitterSlots[index].isEmpty)
        {
            //空格子名字为空
            EmitterName.text = "空槽位";
            HealthBar.gameObject.SetActive(false);
            bulletLeftText.text = "";
            suggestLine.SetActive(false);
            arrowInst.SetActive(false);
        }
        
        
        
        
        //判断是否发射
        if (emitterSlots[index].isLaunched || emitterSlots[index].isEmpty==true)
        {
            LaunchButton.SetActive(false);
        }
        
        //是否开启提示线         
        if (emitterSlots[index].isLaunched)
        {
            HealthBar.value = emitterSlots[index].fuel / emitterSlots[index].fuelTotal;
            suggestLine.SetActive(true);
            arrowInst.SetActive(true);
        }
        
    }

    public void InactiveSlot(int index)
    {
        emitterSlots[index].defaultColorSlot();
    }
    
    public EmitterDetails GetEmitterDetails(int ID)
    {
        return emitterDataList.EmitterDataList.Find(i => i.ID == ID);
    }
    
    //画引导线
    private void DrawSuggestLine(Vector3 start, GameObject arrowIcon, Camera camera, LineRenderer line)
    {
        Vector3 mousePos = camera.ScreenToViewportPoint(Input.mousePosition);
        Vector3 startVS = camera.WorldToViewportPoint(start);
        mousePos.z = startVS.z;
        Vector3 end = (mousePos - startVS).normalized +start;
        line.positionCount = 2;
        line.SetPosition(0,start);
        line.SetPosition(1,end);
        arrowIcon.transform.position = end;
        Vector3 arrowDirection = (mousePos - startVS).normalized;
        bulletDirection = arrowDirection;
        arrowIcon.transform.up = arrowDirection;
        
    }

    private IEnumerator FinishGame()
    {
        GameObject finishGameInst=Instantiate(finishGameUIInst,GameObject.Find("Canvas_SpaceBattle").transform);
        yield return new WaitForSeconds(2f);
        Destroy(finishGameInst);
        Controller_Terra.canMoveTerra=true;
        //这里再加一句告诉游戏进度干完了，可以打开另一个Trigger了
        GameProgressManager.Instance.Day2_Work_isFinished = true;
        GameProgressManager.Instance.Day2_Work_Finished();
        TransitionManager.Instance.mainCamera.enabled = true;
        TransitionManager.Instance.Transition("Direct","L2_ControlRoom",TerraControlRoomTransform);
        
        
    }

    public void KillAll()
    {
        killCount = 5;
        Debug.Log("击杀的总数为"+killCount);
    }
    
    
}

