using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmitterSlot : MonoBehaviour
{
    [Header("组件获取")] [SerializeField] private Image slotImage;
    public bool isEmpty=true;
    public EmitterDetails emitterDetails;
    public bool isLaunched = false;

    public void UpadateSlot(EmitterDetails emitter)
    {
        emitterDetails = emitter;
        slotImage.sprite = emitter.icon;
        slotImage.enabled = true;
        isEmpty = false;
    }

    public void ClearSlot()
    {
        slotImage.enabled = false;
        isEmpty = true;
    }

    public void HighLightSlot()
    {
        this.gameObject.GetComponent<Image>().color = Color.white;
        slotImage.color = Color.white;
    }

    public void defaultColorSlot()
    {
        this.gameObject.GetComponent<Image>().color = Color.gray;
        slotImage.color = Color.gray;
    }
}
