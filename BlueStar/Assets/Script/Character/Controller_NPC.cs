using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Controller_NPC : MonoBehaviour
{
    public GameObject armorLine;
    public GameObject bloodLine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        RectTransform rectTransform_armor = armorLine.GetComponent<RectTransform>();
        RectTransform rectTransform_blood =bloodLine.GetComponent<RectTransform>();
        if (other.gameObject.CompareTag("Bullet"))
        {
            if (rectTransform_armor.sizeDelta.x>0)
            {
                float currentWidth = rectTransform_armor.sizeDelta.x - 4;
                rectTransform_armor.sizeDelta = new Vector2(currentWidth, rectTransform_armor.sizeDelta.y);
            }
            else
            {
                float currentWidth = 0;
                rectTransform_blood.sizeDelta = new Vector2(currentWidth, rectTransform_blood.sizeDelta.y);
            }
            
        }

    }
}
