using System;
using System.Collections;
using System.Collections.Generic;
using BlueStar.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProgressManager : Singleton<GameProgressManager>
{
    [Header("声音")] [SerializeField] private AudioClip[] _audioClips;
    private AudioSource audio;
    [Header("主相机")] [SerializeField] private Camera mainCamera;
    public GameObject Terra;
    [Header("Day1_RepairFinish")] public bool isRepairFinish = false;
    [Header("Day2_Zombie_Die")]
    public bool Day2_Zombie_isAlive = true;
    [Header("Day2_Work")]
    public bool Day2_Work_isFinished=false;
    public GameObject Timeline_UnknownPersonLeaveSuggest;
    [Header("结局")] public bool enterEndingSHow;
    [Header("游戏任务提示UI")] public GameObject taskSuggest;
    public TMP_Text TaskText;

    private void Start()
    {
        audio = mainCamera.gameObject.GetComponent<AudioSource>();
        audio.clip = _audioClips[0];
        audio.Play();
        taskSuggest.SetActive(false);
        TaskText.text = "找到损坏的ID卡, 制作新的宿舍通行证";
    }

    public void Day1_BeginRepair()
    {
        TaskText.text = "装备配枪, 找到配电间, 拿走电阻, 替换损坏的电阻并调节电流";
    }



    public void Day1_FinishRepair()
    {
        isRepairFinish = true;
        Day2_Zombie_isAlive = false;
        Terra.transform.position = new Vector3(54f, 0.1f, -12f);
        StartCoroutine(LoadDay2Scene());
        

    }

    public IEnumerator LoadDay2Scene()
    {
        AsyncOperation loadTrainingCourse = SceneManager.LoadSceneAsync("L2_TrainingCourse", LoadSceneMode.Additive);
        yield return loadTrainingCourse;
        SceneManager.UnloadSceneAsync("L1_Shaft");
        audio.clip = _audioClips[1];
        audio.Play();
        TaskText.text = "与指挥官交谈, 开启新一天的任务";
        TransitionManager.Instance.Map_L2.SetActive(true);
        TransitionManager.Instance.Map_L1.SetActive(false);
        TransitionManager.Instance.Map_L4.SetActive(false);
    }

    public void BeginAttackSignal()
    {
        TaskText.text = "前往对应的控制室开启信号打击任务";
    }

    public void Day2_Work_Finished()
    {
        if (Day2_Work_isFinished)
        {
            InventoryManager.Instance.suggestGlobal.SetActive(true);
            Timeline_UnknownPersonLeaveSuggest.SetActive(true);
            InventoryStateManager.Instance.DoorStates["Door_ControlRoom_wing"]=true;
            InventoryStateManager.Instance.DoorStates["Door_TrainingCourse_To_Decompression_wing"]=true;
            InventoryStateManager.Instance.DoorStates["Door_Corridor_1_wing"]=true;
            InventoryStateManager.Instance.DoorStates["O_I_LiftDoor"] = true;
            Debug.Log("游戏进程让O_I_LiftDoor的值为"+InventoryStateManager.Instance.DoorStates["O_I_LiftDoor"]);
            TaskText.text = "乘坐电梯前往天台寻找档案室入口";
            
        }
    }

    public void Day2_GotToLaunch()
    {
        TaskText.text = "前往二层飞船调度室调度飞船前往荒星";
    }

    public void Day2_Zombie_Die()
    {
        
    }

    public void EnterEndingShow()
    {
        if (enterEndingSHow)
        {
            audio.clip = _audioClips[2];
            audio.Play();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Day2_Work_isFinished = true;
            Debug.Log("完成了第二天的工作");
            Day2_Work_Finished();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            taskSuggest.SetActive(!taskSuggest.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("HomePage");
            EventHandler.CallDestroyObject(true);
        }
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
        Destroy(Terra.gameObject);
        Destroy(this.gameObject);
    }
}
