using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineUI : MonoBehaviour
{
    private GameObject infoUI;
    public static GameObject infoUIInst;
    public static TMP_Text infoText;
    public static int infoUICurrentIndex=0;
    public static int currentPlayIndex=-1;
    public static string[] UIInfos;
    public static int[] playIndexs;
    

    private void Start()
    {
        infoUI=Resources.Load("Prefabs/UI/TimelineInfos") as GameObject;
    }

    private void OnEnable()
    {
        EventHandler.ShowTimelineInfos += onShowTimelineInfos;
    }

    private void OnDisable()
    {
        EventHandler.ShowTimelineInfos -= onShowTimelineInfos;
    }

    /// <summary>
    /// 生成提示UI的方法
    /// </summary>
    /// <param name="infos"></param>
    /// <param name="indexs"></param>
    void onShowTimelineInfos(string[] infos, int[] indexs)
    {
        UIInfos = infos;
        playIndexs = indexs;
        if (playIndexs.Length>0)
        {
            currentPlayIndex = playIndexs[0];
        }
        infoUIInst=Instantiate(infoUI, GameObject.Find("------UI------/UI_2D").gameObject.transform);
        infoUIInst.SetActive(false);
        infoText = infoUIInst.transform.Find("Text").GetComponent<TMP_Text>();
        if (infos != null)
        {
            infoUICurrentIndex = 0;
            infoText.text = UIInfos[infoUICurrentIndex];
        }
        else
        {
            infoText.text = "";
        }
    }

    // timeline播放的时候没有禁用字幕；只在stop的时候出现；当放到某个index的按钮就继续播放
    /// <summary>
    /// 按钮绑定的方法，点击后更新UI信息
    /// </summary>
    public void UpdateTimelineInfos()
    {
        if (infoUICurrentIndex < UIInfos.Length)
        {
            infoUICurrentIndex += 1;
            TimelineUI.infoText.text = UIInfos[infoUICurrentIndex];
            if (currentPlayIndex>=0 && infoUICurrentIndex==currentPlayIndex+1)
            {
                //当继续播放的序号等于当前句子的序号；通知director继续播放,并关闭显示字幕
                TimelineTrigger.currentObj.GetComponent<PlayableDirector>().Play();
                TimelineUI.infoUIInst.SetActive(false);
                if (Array.IndexOf(playIndexs,currentPlayIndex)<UIInfos.Length-1)
                {
                    Debug.Log("playIndex:"+currentPlayIndex);
                    currentPlayIndex = playIndexs[Array.IndexOf(playIndexs, currentPlayIndex) + 1];
                }
                else
                {
                    currentPlayIndex = -1;
                }
            }
        }
        else if (infoUICurrentIndex >= UIInfos.Length)
        {
            TimelineTrigger.currentObj.GetComponent<PlayableDirector>().Play();
            Destroy(TimelineUI.infoUIInst.gameObject);
            TimelineUI.infoUICurrentIndex = 0;
        }


    }
    
    /// <summary>
    /// Timeline停止播放时激活底部信息
    /// </summary>
    public void EnableTimelineInfos()
    {
        if (TimelineUI.infoUIInst!=null)
        {
            infoUIInst.SetActive(true);
        }
    }
    
    
}
