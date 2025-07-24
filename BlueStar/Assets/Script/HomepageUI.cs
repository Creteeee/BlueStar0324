using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class HomepageUI : MonoBehaviour
{
    public GameObject camera;
    public GameObject HomepageUI1;
    public GameObject HomepageUI2;
    public GameObject HomepageUI3;
    public Transform transform1;
    public Transform transform2;
    public GameObject button1;
    public GameObject botton2;
    public PlayableDirector Director;
    public Material bloodMat;
    public Volume volume;
    private ColorAdjustments colorAdjustments;
  

    private void Awake()
    {
        Director = this.GetComponent<PlayableDirector>();
        bloodMat.SetFloat("_Alpha",0);
        bloodMat.SetFloat("_HealthRatio",1);
        volume.sharedProfile.TryGet(out colorAdjustments);
        colorAdjustments.colorFilter.value=Color.white;

    }

    public void NextUI1()
    {
        HomepageUI2.SetActive(true);
        button1.gameObject.SetActive(false);
        camera.transform.DOMove(transform1.position, 2f);
        camera.transform.DORotate(transform1.rotation.eulerAngles, 2);

    }
    
    public void NextUI2()
    {
      
        HomepageUI2.gameObject.SetActive(false);
        camera.transform.DOMove(transform2.position, 2f);
        camera.transform.DORotate(transform2.rotation.eulerAngles, 2);
        HomepageUI3.gameObject.SetActive(true);

    }
    
    public void NextUI3()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        StartCoroutine(LoadScenesAndStartGame());
    }

    private IEnumerator LoadScenesAndStartGame()
    {
        // 1. 加载主逻辑场景
        AsyncOperation loadPersistent = SceneManager.LoadSceneAsync("PersistantLevel", LoadSceneMode.Additive);
        yield return loadPersistent;
        
        AsyncOperation loadHome = SceneManager.LoadSceneAsync("L2_Home", LoadSceneMode.Additive);
        yield return loadHome;

        // 2. 加载目标场景 O_OverView
        AsyncOperation loadOverview = SceneManager.LoadSceneAsync("O_OverView", LoadSceneMode.Additive);
        yield return loadOverview;

        // 3. 设置为激活场景
        Scene overviewScene = SceneManager.GetSceneByName("O_OverView");
        if (overviewScene.IsValid() && overviewScene.isLoaded)
        {
            SceneManager.SetActiveScene(overviewScene);
        }
        else
        {
            Debug.LogError("O_OverView 场景无效或未加载完成！");
        }

        // 4. 卸载当前场景
        SceneManager.UnloadSceneAsync("HomePage");
    }

    public void playTimeLine()
    {
        Director.Play();
    }

}