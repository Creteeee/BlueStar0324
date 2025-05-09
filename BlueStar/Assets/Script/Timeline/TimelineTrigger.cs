using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.UI;

public class TimelineTrigger : MonoBehaviour
{
    private PlayableDirector director;
    public static GameObject currentObj;
    [Header("Timeline字幕")] [SerializeField]private string[] infos;
    [Header("继续播放的序号")] [SerializeField] private int[] indexs;
    private TimelineUI timelineUI;
    public GameObject animationCamera;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        director = GetComponent<PlayableDirector>();
        timelineUI=GameObject.Find("------TimelineUI------").GetComponent<TimelineUI>();
        //animationCamera = GameObject.Find("------Camera------/AnimationCamera").gameObject; 手动指定
        //animationCamera.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player" )
        {
            Debug.Log("检测到Player");
            if (director!=null)
            {
                currentObj=this.gameObject;
                EventHandler.CallShowTimelineInfos(infos,indexs);
                Debug.Log("呼叫生成了Timeline");
                EventHandler.CallMoveHeader(true);
                director.Play();
                Controller_Terra.canMoveTerra = false;
            }
        }
    }

    public void EndTimeline()
    {
        this.gameObject.GetComponent<TimelineTrigger>().enabled = false;
        this.gameObject.GetComponent<Collider>().enabled = false;
        EventHandler.CallResetHeader(true);
        animationCamera.SetActive(false);
        Controller_Terra.canMoveTerra = true;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// StopSignal绑定的方法，停止Timeline，并且呼叫UI开启字幕
    /// </summary>
    public void StopTimeline()
    {
        director.Pause();
        //呼叫TimelineUI显示字幕
        timelineUI.EnableTimelineInfos();
        
    }
}
