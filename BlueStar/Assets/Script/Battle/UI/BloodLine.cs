using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class BloodLine : MonoBehaviour
{
    public GameObject bloodline;
    // Start is called before the first frame update
    void OnEnable()
    {
        EventCenter_BattleMode.HitEnemy += ReduceBlood;
    }
    
    void OnDisable()
    {
        EventCenter_BattleMode.HitEnemy -= ReduceBlood;
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReduceBlood()
    {
        RectTransform rectTransform = bloodline.GetComponent<RectTransform>();

        // 获取当前的宽度
        float currentWidth = rectTransform.sizeDelta.x - 4;

        // 设置新的宽度，假设你要设置为 12
        rectTransform.sizeDelta = new Vector2(currentWidth, rectTransform.sizeDelta.y);
    }
}
