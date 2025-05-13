using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSearchingSystem : MonoBehaviour
{
    public GameObject SerchingSystemUI;
    public GameObject SuggestE;
    private bool isEnter = false;

    void Start()
    {
        SuggestE.SetActive(false);
    }

    void Update()
    {
        if (isEnter)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SerchingSystemUI.SetActive(true);
            }
        }
        
    }
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
}
