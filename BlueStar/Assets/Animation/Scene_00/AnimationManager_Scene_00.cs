using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;


public class AnimationManager_Scene_00 : MonoBehaviour
{
    public DialogueManager dialogManager;
    public GameObject darkPlane;
    public Material SunLightMat;
    public GameObject Terra;
    public PlayableDirector director1;
    public PlayableDirector director2;
    public AnimationTrack Track_Terra1;
    public AnimationTrack Track_Terra2;
    public GameObject focusCamera;
    public GameObject MainCamera;
    public Transform[] focusPoses;
    private Dictionary<string, int> posDic = new Dictionary<string, int>();
    private int timer=0;
    


    private void Start()
    {
        SunLightMat.SetFloat("_isSunlight",0f);
        TimelineAsset timeline1 = director1.playableAsset as TimelineAsset;
        TimelineAsset timeline2 = director2.playableAsset as TimelineAsset;
        Terra.SetActive(false);
        MainCamera=GameObject.Find("Main Camera");
        foreach (var track in timeline1.GetOutputTracks())
        {
            if (track.name =="Terra")
            {
                Track_Terra1 = track as AnimationTrack;
                if (Track_Terra1 != null)
                {
                    Debug.Log("成功找到并赋值 Terra AnimationTrack");
                }
                else
                {
                    Debug.LogWarning("找到 Terra 轨道，但无法转换为 AnimationTrack");
                }
            }
        }
        foreach (var track in timeline2.GetOutputTracks())
        {
            if (track.name =="Terra")
            {
                Track_Terra2 = track as AnimationTrack;
                if (Track_Terra2 != null)
                {
                    Debug.Log("成功找到并赋值 Terra AnimationTrack");
                }
                else
                {
                    Debug.LogWarning("找到 Terra 轨道，但无法转换为 AnimationTrack");
                }
            }
        }
        director1.SetGenericBinding(Track_Terra1, null);
        director2.SetGenericBinding(Track_Terra2, null);
        focusCamera.SetActive(false);
        posDic["hand"] = 0;

    }


    // Update is called once per frame
    void Update()
    {
        if (dialogManager.dialogIndex==3)
        {
            SunLightMat.SetFloat("_isSunlight",1f);
            Debug.Log("现在讲到第3句了");
        }
        
        if (dialogManager.dialogIndex == 9)
        {
            Debug.Log( "现在讲到第9句了");
            darkPlane.SetActive(false);
            Terra.SetActive(true);
            director1.SetGenericBinding(Track_Terra1, Terra);
            director1.Play();
        }

        if (dialogManager.dialogIndex == 12)
        {
            // 停止 timeline1 并清除绑定
            director1.Stop();
            director1.SetGenericBinding(Track_Terra1, null);
            Terra.SetActive(false);// 清除绑定
            director1.time = 0;  // 重置时间轴

            // 设置并播放 timeline2
            Terra.SetActive(true);
            director2.SetGenericBinding(Track_Terra2, Terra);  // 确保 Terra 绑定到 timeline2
            director2.Play();  // 播放 timeline2
            focusCamera.SetActive(true);
            focusCamera.gameObject.transform.position = focusPoses[posDic["hand"]].position;
            
            

            


        }
        if (dialogManager.dialogIndex == 14)
        {
            focusCamera.SetActive(false);
            
        }

        if (dialogManager.dialogIndex > 18)
        {
            if (timer<1)
            {
                MainCamera.transform.DOMoveY(2f,0.1f).SetLoops(-1, LoopType.Yoyo);
                
                timer += 1;
            }
            
        
        }

        if (dialogManager.dialogIndex == 23)
        {
            SceneManager.LoadScene(1);
        }

        MainCamera.transform.position +=new Vector3(0,0,Input.GetAxis("Vertical")*(-0.001f));
    }
    
    
}
