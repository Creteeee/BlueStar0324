using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EmitterSlot : MonoBehaviour
{
    [Header("组件获取")] [SerializeField] private Image slotImage;
    public bool isEmpty=true;
    public EmitterDetails emitterDetails;
    public bool isLaunched = false;//这个值注意飞船销毁时要便会false
    public GameObject Emitter_Launched;
    public GameObject SpaceShip;

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

    public void LaunchEmitter()
    {
        //在飞船处实例化发射的模型，后面加上朝向
        Emitter_Launched = Instantiate(emitterDetails.model_Orbit, SpaceShip.transform.position,Quaternion.identity);
        isLaunched = true;
    }
}
