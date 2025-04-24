using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmitterSlot : MonoBehaviour
{
    [Header("组件获取")] [SerializeField] private Image slotImage;
    public bool isEmpty=true;
    public EmitterDetails emitterDetails;
    public bool isLaunched = false;//这个值注意飞船销毁时要便会false
    public GameObject Emitter_Launched;
    public GameObject SpaceShip;
    private GameObject bulletPrefab;
    
    //飞船的状态
    public float fuelTotal;
    public float bulletTotal;
    public float duration;
    public float fuel;
    public float bullet;

    //prelook模型们，虽然这样写有点傻，但是就这样吧
    public GameObject[] prelookModels;
    
    

    private void Update()
    {
        if (isLaunched)
        {
            if (fuel==0 || bullet==0)
            {
                ClearSlot();
            }
            
        }

    }

    public void UpadateSlot(EmitterDetails emitter)
    {
        emitterDetails = emitter;
        slotImage.sprite = emitter.icon;
        slotImage.enabled = true;
        isEmpty = false;
        
        //更新燃料条和血量
        fuelTotal = emitterDetails.health;
        fuel = fuelTotal;
        bulletTotal = emitterDetails.bulletLeft;
        bullet=bulletTotal;
        duration=emitterDetails.duration;
        bulletPrefab = emitterDetails.bulletPrefab;
    }

    public void ClearSlot()
    {
        slotImage.enabled = false;
        isEmpty = true;
        isLaunched = false;
        prelookModels[emitterDetails.ID].SetActive(false);
        emitterDetails=null;
        Destroy(Emitter_Launched.gameObject.GetComponent<Emitter>().line);
        Destroy(Emitter_Launched.gameObject);
        UIManager_BattleMode.suggestLine.SetActive(false);
        UIManager_BattleMode.arrowInst.SetActive(false);
        Emitter_Launched=null;
        fuelTotal = 0;
        bulletTotal = 0;
        fuel = 0;
        bullet = 0;
        duration = 0;
        UIManager_BattleMode.HealthBar.gameObject.SetActive(false);
        UIManager_BattleMode.bulletLeftText.text="";
        UIManager_BattleMode.EmitterName.text="空槽位";
    }

    public void HighLightSlot()
    {
        this.gameObject.GetComponent<Image>().color = Color.white;
        slotImage.color = Color.white;
        if (isLaunched)
        {
            Emitter_Launched.transform.Find("Canvas").GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    public void defaultColorSlot()
    {
        this.gameObject.GetComponent<Image>().color = Color.gray;
        slotImage.color = Color.gray;
        if (isLaunched)
        {
            Emitter_Launched.transform.Find("Canvas").GetComponent<CanvasGroup>().alpha = 0;
        }
        
    }

    public void LaunchEmitter()
    {
        //在飞船处实例化发射的模型，后面加上朝向
        Emitter_Launched = Instantiate(emitterDetails.model_Orbit, SpaceShip.transform.position,Quaternion.identity);
        StartCoroutine(UpdateFuel(fuelTotal, duration));
        isLaunched = true;
    }

    public IEnumerator UpdateFuel(float fuelTotal,float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime<duration)
        {
            elapsedTime+=Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime/duration);
            fuel=Mathf.Lerp(fuelTotal,0,t);
            yield return null;
        }
        
    }

    public void Shoot()
    {
        if (bullet>0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
      
                GameObject obj = Instantiate(bulletPrefab, Emitter_Launched.transform.position, Quaternion.identity);
                obj.transform.up = UIManager_BattleMode.bulletDirection;
                bullet -= 1;
                UIManager_BattleMode.bulletLeftText.text = bullet.ToString() + "/" + bulletTotal;
            }
        }
    }
    

    
}
