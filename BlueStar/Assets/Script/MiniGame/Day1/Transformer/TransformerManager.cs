using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TransformerManager : MonoBehaviour
{
    private float R1;
    private float R2;
    private float R3;
    public TMP_Text R1_Text;
    public TMP_Text R2_Text;
    public TMP_Text R3_Text;

    private float I1;
    private float I2;
    private float I3;
    public TMP_Text I1_Text;
    public TMP_Text I2_Text;
    public TMP_Text I3_Text;
    public GameObject PlayerTrigger;

    public PlayableDirector directer;

    private void Awake()
    {
        PlayerTrigger.SetActive(false);
        R1 = 1;
        R2 = 1;
        R3 = 1;
        R1_Text.text = R1.ToString()+"Ω";
        R2_Text.text = R2.ToString()+"Ω";
        R3_Text.text = R3.ToString()+"Ω";

        I1 = 53;
        I2 = 53;
        I3 = 160;
        I1_Text.text = I1.ToString() + "A";
        I2_Text.text = I2.ToString() + "A";
        I3_Text.text = I3.ToString() + "A";
    }

    private void Update()
    {
        UpdateResistance();

    }

    public void UpdateTransformer()
    {
        R1_Text.text = R1.ToString()+"Ω";
        R2_Text.text = R2.ToString()+"Ω";
        R3_Text.text = R3.ToString()+"Ω";
        Debug.Log("未四舍五入的电流是" + (R2 / (R1 + R2 + R1 * R2)) * 160);
        I1 = Mathf.RoundToInt((R2 / (R1 + R2 + R1 * R2)) * 160);
        Debug.Log("四舍五入的电流是" +I1);
        I2 = Mathf.RoundToInt((R1 / (R1 + R2 + R1 * R2) )* 160);
        I3 = Mathf.RoundToInt(160 / R3);
        I1_Text.text = I1.ToString() + "A";
        I2_Text.text = I2.ToString() + "A";
        I3_Text.text = I3.ToString() + "A";
        if (I1 == 40 && I2 == 40 && I3 == 40) 
        {
            
            // EventHandler.CallMoveHeader(true);
            //this.GetComponent<TimelineTrigger>().enabled = true;
            SceneManager.LoadScene("L2_Electricity_01",LoadSceneMode.Additive);
            SceneManager.LoadScene("L2_Decompression_2",LoadSceneMode.Additive);
            SceneManager.LoadScene("L2_TrainingCourse",LoadSceneMode.Additive);
            PlayerTrigger.SetActive(true);
            
        }
        
    }

    public void UpdateResistance()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask("Click")))
            {
                switch (hit.transform.name)
                {
                    case "Up_1":
                        R1 += 1;
                        break;
                    case "Up_2":
                        R2 += 1;
                        break;
                    case "Up_3":
                        R3 += 1;
                        break;
                    case "Down_1":
                        if (R1>1)
                        {
                            R1 -= 1;
                        }
                        break;
                    case "Down_2":
                        if (R2>1)
                        {
                            R2 -= 1; 
                        }
                        break;
                    case "Down_3":
                        if (R3>1)
                        {
                            R3 -= 1;
                        }
                        break;
                }
                UpdateTransformer();
                
            }


            
        }
    }

}
