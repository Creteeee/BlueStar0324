using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        audio = mainCamera.gameObject.GetComponent<AudioSource>();
        audio.clip = _audioClips[0];
        audio.Play();
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
    }

    public void Day2_Work_Finished()
    {
        if (Day2_Work_isFinished)
        {
            Timeline_UnknownPersonLeaveSuggest.SetActive(true);
            InventoryStateManager.Instance.DoorStates["Door_ControlRoom_wing"]=true;
            InventoryStateManager.Instance.DoorStates["Door_TrainingCourse_To_Decompression_wing"]=true;
            InventoryStateManager.Instance.DoorStates["Door_Corridor_1_wing"]=true;
            InventoryStateManager.Instance.DoorStates["O_I_LiftDoor"] = true;
            Debug.Log("游戏进程让O_I_LiftDoor的值为"+InventoryStateManager.Instance.DoorStates["O_I_LiftDoor"]);
            
        }
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
    }
}
