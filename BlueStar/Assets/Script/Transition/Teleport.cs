using System;
using System.Collections;
using System.Collections.Generic;
using BlueStar.Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    [Header("场景信息")]
    public string SceneFrom;
    public string SceneTo;
    private Scene scene1;
    private Scene scene2;
    [Header("玩家的新位置")] public Transform transform;

    private void Awake()
    {
        scene1 = SceneManager.GetSceneByName(SceneFrom);
        scene2 = SceneManager.GetSceneByName(SceneTo);       
    }

    public void onTransitionToScene()
    {
        Debug.Log("Teleport被调用了");
        if (InventoryManager.Instance != null)
        {
            Debug.Log("InventoryManager不为空");
        }

        if (TransitionManager.Instance == null)
        {
            Debug.Log("TransitionManager为空");
        }
        TransitionManager.Instance.Transition(SceneFrom,SceneTo,transform);
    }
}
