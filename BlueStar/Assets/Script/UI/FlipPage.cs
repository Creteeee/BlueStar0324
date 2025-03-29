using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipPage : MonoBehaviour
{
    public GameObject[] pages;
    public int count;
    public int index = 0;

    private void Start()
    {
        count = pages.Length;
    }

    public void FlipToNext()
    {
        if (index!=count-1)
        {
            pages[index].SetActive(false);
            index += 1;
            pages[index].SetActive(true);
        }
    }

    public void FlipToPrevious()
    {
        if (index != 0)
        {
            pages[index].SetActive(false);
            index -= 1;
            pages[index].SetActive(true);
        }
    }

    private void OnDisable()
    {
        index = 0;
    }

    private void OnEnable()
    {
        for (int i = 0; i < count; i++)
        {
            pages[i].SetActive(false);
        }
        pages[index].SetActive(true);
    }
}
