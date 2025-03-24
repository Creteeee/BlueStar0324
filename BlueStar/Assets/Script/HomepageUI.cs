using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
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

}