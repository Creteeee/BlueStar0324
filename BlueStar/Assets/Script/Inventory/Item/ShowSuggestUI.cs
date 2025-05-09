using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSuggestUI : MonoBehaviour
{
    public GameObject suggestUI;

    private void Start()
    {
        if (suggestUI!=null)
        {
            suggestUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (suggestUI!=null)
            {
                suggestUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (suggestUI!=null)
            {
                suggestUI.SetActive(false);
            }
        }
    }
}
