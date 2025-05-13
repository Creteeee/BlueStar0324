using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenLaunchPlatformSystem : MonoBehaviour
{
    [Header("2DUI")]
    public GameObject OpenLaunchPlatformUI;
    public GameObject Code1;
    public GameObject WrongSuggest1;
    public GameObject Code2;
    public GameObject WrongSuggest2;
    public GameObject SuccsessSuggest;
    public TMP_Text inputDisplay1;
    public TMP_Text inputDisplay2;
    public string correctPassword1;
    public string correctPassword2;

    private string currentInput1="";
    private string currentInput2="";

    [Header("其他挂载")] public GameObject SuggestE;
    private bool isEnter = false;
    public Door_02 door;
    private int timer1 = 0;
    private int timer2 = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SuggestE.SetActive(true);
            isEnter = true;
            
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SuggestE.SetActive(false);
            isEnter = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&& isEnter&&!OpenLaunchPlatformUI.activeSelf)
        {
            OpenLaunchPlatformUI.SetActive(true);
        }
       

        if (Input.GetKeyDown(KeyCode.Escape) && OpenLaunchPlatformUI.activeSelf)
        {
            OpenLaunchPlatformUI.SetActive(false);
        }
   

        if (Code1.activeSelf)
        {
            for (KeyCode k = KeyCode.Alpha0; k <= KeyCode.Alpha9; k++)
            {
                if (Input.GetKeyDown(k))
                {
                    string digit = ((int)k - (int)KeyCode.Alpha0).ToString();
                    AddDigit1(digit);
                    break;
                }
            }
        }

        if (Code2.activeSelf)
        {
            for (KeyCode k = KeyCode.Alpha0; k <= KeyCode.Alpha9; k++)
            {
                if (Input.GetKeyDown(k))
                {
                    string digit = ((int)k - (int)KeyCode.Alpha0).ToString();
                    AddDigit2(digit);
                    break;
                }
            }
            
        }
        
        
    }

    void AddDigit1(string digit)
    {
        if (currentInput1.Length >= 4)
            return;

        currentInput1 += digit;
        UpdateDisplay1();

        if (currentInput1.Length == 4)
        {
            CheckPassword1();
        }
    }
    void AddDigit2(string digit)
    {
        if (currentInput2.Length >= 4)
            return;

        currentInput2 += digit;
        UpdateDisplay2();

        if (currentInput2.Length == 4)
        {
            CheckPassword2();
        }
    }
    void CheckPassword1()
    {
        if (currentInput1 == correctPassword1 && timer1==0)
        {
            Debug.Log("密码正确！");
            Code2.SetActive(true);
            Code1.SetActive(false);
            currentInput2 = "";
            inputDisplay2.text = "";
            timer1 = 1;
            // 触发成功事件，比如开门、激活动画等
        }
        else
        {
            Debug.Log(" 密码错误！");
            // 重置输入，或者提示用户
            WrongSuggest1.SetActive(true);
        }

        // 重置输入内容（可选，或延迟清除）
        currentInput1 = "";
        UpdateDisplay1();
    }
    void CheckPassword2()
    {
        if (currentInput2 == correctPassword2 && timer2==0)
        {
            Debug.Log("密码正确！");
            Code2.SetActive(false);
            SuccsessSuggest.SetActive(true);
            WrongSuggest2.SetActive(false);
            // 触发成功事件，比如开门、激活动画等
            timer2 = 1;
            door.canOpen = true;
        }
        else
        {
            Debug.Log(" 密码错误！");
            // 重置输入，或者提示用户
            WrongSuggest2.SetActive(true);
        }

        // 重置输入内容（可选，或延迟清除）
        currentInput2 = "";
        UpdateDisplay2();
    }
    
    void UpdateDisplay1()
    {
        inputDisplay1.text = currentInput1;
    }
    void UpdateDisplay2()
    {
        inputDisplay2.text = currentInput2;
    }
}
