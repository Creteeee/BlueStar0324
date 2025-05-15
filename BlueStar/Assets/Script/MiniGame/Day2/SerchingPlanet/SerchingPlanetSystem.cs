using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SerchingPlanetSystem : MonoBehaviour
{
    public GameObject HomePageUI;
    public GameObject SerchingPlanetUI;
    public GameObject SerchingResaultUI;
    public GameObject WrongIndexSuggest;
    public GameObject MessageRecordUI;
    public string correctPassword;
    private string currentInput = "";
    public TMP_Text inputDisplay;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)||Input.GetKeyDown(KeyCode.Escape))
        {
            HomePageUI.SetActive(true);
            SerchingPlanetUI.SetActive(false);
            SerchingResaultUI.SetActive(false);
            WrongIndexSuggest.SetActive(false);
            MessageRecordUI.SetActive(false);
            this.gameObject.SetActive(false);
        }
        if (SerchingPlanetUI.activeSelf)
        {
    
            for (KeyCode k = KeyCode.Alpha0; k <= KeyCode.Alpha9; k++)
            {
                if (Input.GetKeyDown(k))
                {
                    string digit = ((int)k - (int)KeyCode.Alpha0).ToString();
                    AddDigit(digit);
                    break;
                }
            }
        }
    }
    void UpdateDisplay()
    {
        inputDisplay.text = currentInput;
    }
    void AddDigit(string digit)
    {
        if (currentInput.Length >= 4)
            return;

        currentInput += digit;
        UpdateDisplay();

        if (currentInput.Length == 4)
        {
            CheckPassword();
        } 
    }
    void CheckPassword()
    {
        if (currentInput == correctPassword)
        {
            Debug.Log("密码正确！");
            SerchingResaultUI.SetActive(true);
            SerchingPlanetUI.SetActive(false);
            // 触发成功事件，比如开门、激活动画等
            GameProgressManager.Instance.Day2_GotToLaunch();
        }
        else
        {
            Debug.Log(" 密码错误！");
            // 重置输入，或者提示用户
            WrongIndexSuggest.SetActive(true);
        }

        // 重置输入内容（可选，或延迟清除）
        currentInput = "";
        UpdateDisplay();
    }

    public void enableSerchingPlanet(SerchingPlanetSystem system)
    {
        system.SerchingPlanetUI.SetActive(true);
        system.HomePageUI.SetActive(false);
    }

    public void enableMessageRecord(SerchingPlanetSystem system)
    {
        system.MessageRecordUI.SetActive(true);
        system.HomePageUI.SetActive(false);
    }

    public void gobacktoHomepage(SerchingPlanetSystem system)
    {
        system.HomePageUI.SetActive(true);
        system.SerchingPlanetUI.SetActive(false);
        system.SerchingResaultUI.SetActive(false);
        system.WrongIndexSuggest.SetActive(false);
        system.MessageRecordUI.SetActive(false);

    }
}
