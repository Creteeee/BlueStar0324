using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ChangeIcon : MonoBehaviour
{
    private Sprite sprite;
    public int EmitterType=0;
    
    void Start()
    {
        sprite = this.GetComponent<Image>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Image>().sprite =DataManager.emitterConfigs[EmitterType].Icon;

    }

    public void onChangeIcon()
    {
        
    }
    
    public void PrepareEmitter(GameObject go)
    {
        
        DataManager.unLaunchedEmittersID.Add(go.GetComponent<ChangeIcon>().EmitterType);
        DataManager.updateUnlaunchedEmitterType = go.GetComponent<ChangeIcon>().EmitterType; 
        EventCenter_BattleMode.NotifyUnlaunchedEmitterUpdated(go.GetComponent<ChangeIcon>().EmitterType);
        go.GetComponent<ChangeIcon>().EmitterType = 0;
        Debug.Log("当前未发射的Emitter个数是"+DataManager.unLaunchedEmittersID.Count);
        Debug.Log("当前未发射的Emitter序号为"+DataManager.updateUnlaunchedEmitterType);
        
    }
}
