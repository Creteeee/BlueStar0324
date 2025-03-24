using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DataManager : MonoBehaviour
{
    private Dictionary<int, EmitterConfigPath> emitterConfigsPaths = new Dictionary<int, EmitterConfigPath>();
    public static Dictionary<int, EmitterConfig> emitterConfigs = new Dictionary<int, EmitterConfig>();//全局访问
    public static List<int> emittersID = new List<int>();
    public static List<int> unLaunchedEmittersID = new List<int>();//未发射的EmitterType的列表
    public static List<int> launchedEmittersID = new List<int>();//存放发射的EmitterType列表
    //--------------------------------这个部分暂时先不做，只做一个Emitter---------------------------
    public static List<GameObject> unLaunchedEmitterModels = new List<GameObject>();//存未发射但实例化的EmitterModel
    public static List<GameObject> launchedEmitterModels = new List<GameObject>();//存发射的EmitterModel
    public static List<int> activatedEmittersID = new List<int>();
    public static int currentEmitterNumber = 1;
    //----------------------------------------------------------------------------------
    public static int updateUnlaunchedEmitterType=0;
    public static int updateLaunchedEmitterType = 0;
    private List<GameObject> Emitterlist = new List<GameObject>();
    
    //----------------------------------存储场景中的Spaceship---------------------------------------
   // public static GameObject spaceship;
    public static GameObject celsestial;
    
    //-----------------------------------暂时控制全局emitterAcceleration-------------------------------------------
    public static float emitterAcceleration = 0f;
    //----------------------------------暂时控制全局的子弹数量------------------------------------------
    public static int bulletCounts =4;
    
    private void OnEnable() 
    {

        Debug.Log("DataManager_BattleMode 已启用");
        
        EventCenter_BattleMode.OnUnlaunchedEmitterUpdated += UpdateUnlaunchedEmitterType;
        //-------------------------加载资源---------------------------------------------
        LoadEmitterConfigsPath("Assets/Script/JSON/EmitterConfig.json");
        LoadEmitterConfig();
        Debug.Log("The number of items in the dictionary: " + emitterConfigsPaths.Count);
        if (emitterConfigs.Count > 0 && emitterConfigs.ContainsKey(0))
        {
            Debug.Log("Emitter字典的第0个元素的Prefab名字是" + emitterConfigs[0].Prefab.name);
        }
        else
        {
            Debug.LogWarning("Emitter字典没有正确加载，第0个元素不存在");
        }
        
        //-----------------------------------------------------------------
        emittersID = unLaunchedEmittersID.Concat(launchedEmittersID).ToList();//总列表等于两个列表的合并
        // spaceship = GameObject.Find("SpaceShip");
        celsestial = GameObject.Find("Celestial");
    }

    private void OnDisable()
    {
        
        EventCenter_BattleMode.OnUnlaunchedEmitterUpdated -= UpdateUnlaunchedEmitterType;
    }

    private void Start()
    {
        


    }

    private void Update()
    {
        emittersID = unLaunchedEmittersID.Concat(launchedEmittersID).ToList();//总列表等于两个列表的合并

    }

    //加载Emitter配置路径的方法
    void LoadEmitterConfigsPath(string filePath)
    {
        string json = System.IO.File.ReadAllText(filePath);

        // 解析 JSON 数据，包装成 Wrapper 类型
        Wrapper<EmitterConfigPath> wrapper = JsonUtility.FromJson<Wrapper<EmitterConfigPath>>(json);

        // 确保 Items 字段不为空
        if (wrapper != null && wrapper.Items != null)
        {
            foreach (var config in wrapper.Items)
            {
                emitterConfigsPaths[config.EmitterType] = config;
            }
        }
        else
        {
            Debug.LogError("Failed to load emitter configs: Invalid JSON format");
        }
    }

    //加载Emitter配置的方法
    void LoadEmitterConfig()
    {
        foreach (var emitterConfigPath in emitterConfigsPaths.Values)
        {
            Debug.Log($"正在加载配置：EmitterType = {emitterConfigPath.EmitterType}");

            // 通过 PrefabPath 加载 Prefab
            GameObject prefab = Resources.Load<GameObject>(emitterConfigPath.PrefabPath);
            if (prefab != null)
            {
                //Debug.Log($"成功加载 Prefab: {emitterConfigPath.PrefabPath}");

                // 创建 EmitterConfig 实例
                EmitterConfig config = new EmitterConfig();
                config.EmitterType = emitterConfigPath.EmitterType;
                config.Prefab = prefab;

                // 通过 IconPath 加载 Icon
                Sprite icon = Resources.Load<Sprite>(emitterConfigPath.IconPath);
                if (icon != null)
                {
                    config.Icon = icon;
                   // Debug.Log($"成功加载 Icon: {emitterConfigPath.IconPath}");
                }
                else
                {
                    //Debug.LogWarning($"未能加载 Icon: {emitterConfigPath.IconPath}");
                }

                // 如果需要，加载模型
                GameObject model = Resources.Load<GameObject>(emitterConfigPath.ModelPath);
                if (model != null)
                {
                    config.Model = model;
                   // Debug.Log($"成功加载 Model: {emitterConfigPath.ModelPath}");
                }
                else
                {
                    Debug.LogWarning($"未能加载 Model: {emitterConfigPath.ModelPath}");
                }

                // 将加载的 EmitterConfig 存入字典
                emitterConfigs[emitterConfigPath.EmitterType] = config;
                //Debug.Log($"成功将 EmitterType {emitterConfigPath.EmitterType} 加入字典");
            }
            else
            {
               // Debug.LogWarning($"未能加载 Prefab: {emitterConfigPath.PrefabPath}");
            }
        }
    }
    

    private void UpdateEmitterList(List<GameObject> emitterlist)
    {

    }

    private void UpdateUnlaunchedEmitterType(int i)
    {
        Debug.Log("我是数据层，我被通知新部署的EmitterType是" + i);
    }

    
}


//Emitter配置路径类
[System.Serializable]
public class EmitterConfigPath
{
    public int EmitterType;
    public string PrefabPath;
    public string IconPath;
    public string ModelPath;
}
//Emitter配置
[System.Serializable]
public class EmitterConfig
{
    public int EmitterType;
    public GameObject Prefab;
    public Sprite Icon;
    public GameObject Model;
}

// 用于包装配置列表
[System.Serializable]
public class Wrapper<T>
{
    public List<T> Items;
}