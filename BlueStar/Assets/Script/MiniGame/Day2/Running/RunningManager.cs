using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class RunningManager : MonoBehaviour
{
    public GameObject Terra;
    public Camera mainCamera;
    public Camera landscapeCamera ;
    public float initialZ;
    private bool isTrigger = false;
    private CanvasGroup blackBG;
    public static bool runningFinish=false;
    private int timer = 0;
    public GameObject AirWall;
    private PlayableDirector _director;

    private void Start()
    {
        blackBG = GameObject.Find("------UI------/UI_2D/BlackBG").gameObject.GetComponent<CanvasGroup>();
        _director = this.GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Controller_Terra.isEndShowState = true;
            other.GetComponent<Animator>().SetFloat("Blend",1);
            Controller_Terra.canMoveTerra = false;
            StartCoroutine(DarkFade());
            mainCamera.enabled = false;
            landscapeCamera.enabled = true;
            initialZ = Terra.transform.position.z;
            isTrigger = true;
            EventHandler.CallMoveHeader(true);
            if (_director!=null)
            {
                _director.Play();
            }
        }
    }

    private void Update()
    {
        if (isTrigger&&!Controller_Terra.canMoveTerra)
        {
            Terra.transform.position= new Vector3(Terra.transform.position.x, Terra.transform.position.y, initialZ);
        }
        
    }

    private IEnumerator DarkFade()
    {
        blackBG.DOFade(1, 0.5f);
        yield return new WaitForSeconds(1f);
        blackBG.DOFade(0, 0.5f);
    }

    public void FinishRunning()
    {
        StartCoroutine(DarkFade());
        mainCamera.enabled = true;
        landscapeCamera.enabled = false;
        AirWall.SetActive(true);
        StartCoroutine(DarkFade());
        EventHandler.CallResetHeader(true);
        Controller_Terra.canMoveTerra = true;
        isTrigger = false;

    }
    
}
