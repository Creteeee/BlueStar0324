using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


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
        
        //MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    //----------------------Emitter发射----------------------------------
    /*private void UpdateLaunchedEmitterID(int ID)
    {
        Emitterlist = emitterlist;
    }*/

    private void Update()
    {
        // Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition); // 从自定义相机发射射线
        // RaycastHit hit;
        //
        // if (Physics.Raycast(ray, out hit,20f))
        // {
        //
        //     // 如果射线击中物体，执行放大操作
        //     if (hit.transform == leftButton.transform)
        //     {
        //         if (Input.GetMouseButton(0))
        //         {
        //             DataManager.emitterAcceleration -=speed;
        //         }
        //
        //     }
        //     else if (hit.transform == rightButton.transform)
        //     {
        //         if (Input.GetMouseButton(0))
        //         {
        //             DataManager.emitterAcceleration +=speed;
        //         }
        //     }
        // }
    }

    public void AddEmitter()
    {
        DataManager.updateLaunchedEmitterType=DataManager.emitterConfigs[DataManager.currentEmitterNumber].EmitterType;  
        EventCenter_BattleMode.NotifyLaunchEmitter(); // 通知事件中心，更新
        Debug.Log($"我是UI层，我发射了新的Emitter");
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
        FuelMenu.SetActive(false);
        FuelProgress.SetActive(true);
        StartCoroutine(UpdatePercentage());

    }
    private IEnumerator UpdatePercentage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 100)
        {
            elapsedTime += Time.deltaTime;
            float percentage = Mathf.Clamp01(elapsedTime / 100) * 100f;
            textFuel.text = $"{Mathf.RoundToInt(percentage)}%";
            yield return null;
        }

        // 确保最终值是 100%
        textFuel.text = "100%";
    }

}

