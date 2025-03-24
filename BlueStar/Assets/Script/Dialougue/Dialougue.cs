using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialougue : MonoBehaviour
{
    [Header("Dialougue开始的编号")] 
    [SerializeField] private int startIndex = 0;
    [SerializeField] private GameObject suggestE;
    private DialogueManager dialougueManager;
    private bool isTriggered = false;

    private void Start()
    {
        if (gameObject.transform.Find("DialogueManager") == null)
        {
            Debug.Log("Manager为空");
        }
        dialougueManager=GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        suggestE.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isTriggered)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DialogueManager.currentDialogueBeginID=startIndex;
                dialougueManager.Awake();
                dialougueManager.Start();
                isTriggered = false;
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            suggestE.gameObject.SetActive(true);
            isTriggered=true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            suggestE.gameObject.SetActive(false);
            isTriggered=false;
        }
    }
   
}
