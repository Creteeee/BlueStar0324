using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class CardWriter : MonoBehaviour
{
    [SerializeField] private int CorrectCode;
    [SerializeField] private List<int> InputCode = new List<int>();
    [SerializeField] private TMP_Text PreText;
    [SerializeField] private TMP_Text CodeText;
    [SerializeField] private PlayableDirector _director;

    private void OnEnable()
    {

    }

    private void Update()
    {
        ClickButton();
        Confirm();
    }

    void ClickButton()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从自定义相机发射射线
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Click"))&& hit.transform.gameObject.CompareTag("CardWriterNumber"))
            {
                PreText.text = "请输入六位身份ID:";
                Debug.Log(hit.transform.name);
                if (InputCode.Count<6)
                {
                    InputCode.Add(int.Parse(hit.transform.gameObject.name));
                    CodeText.text = string.Join("", InputCode); 
                }
            }
        }

    }

    void Confirm()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从自定义相机发射射线
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Click")))
            {
                if (hit.transform.gameObject.name == "Confirm")
                {
                    if (int.Parse(CodeText.text)==CorrectCode)
                    {
                        PreText.text = "打印成功, 请取出卡片";
                        CodeText.text = " ";
                        EventHandler.CallMoveHeader(true);
                        _director.Play();
                        this.gameObject.GetComponent<CardWriter>().enabled = false;
                    }
                    else
                    {
                        CodeText.text = "";
                        InputCode.Clear(); 
                        PreText.text = "密码错误, 请重新输入";
                    }
                }
            }
        }
    }

    public void StopTimeLine()
    {
        EventHandler.CallResetHeader(true);
    }

}
