using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComputerInterection : MonoBehaviour
{
    public GameObject[] interactionUI; 
    public string playerTag = "Player";  
    private bool isPlayerInRange = false;  // 用来标记玩家是否在触发器区域内

    private void Start()
    {
        // 确保 Interaction UI 初始为隐藏状态
        if (interactionUI != null)
        {
            foreach (GameObject item in interactionUI)
            {
                item.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("请在 Inspector 中分配 Interaction UI 对象！");
        }
    }

    private void Update()
    {
        // 如果玩家进入触发器并且按下了 E 键，加载场景
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(3);  // 切换到场景 3
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是 Player 角色进入
        if (other.CompareTag(playerTag) && interactionUI != null)
        {
            foreach (GameObject item in interactionUI)
            {
                item.SetActive(true);
            }
              // 显示交互UI
            isPlayerInRange = true;  // 设置玩家在范围内
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 检查是否是 Player 角色离开
        if (other.CompareTag(playerTag) && interactionUI != null)
        {
            foreach (GameObject item in interactionUI)
            {
                item.SetActive(false);
            }
            isPlayerInRange = false;  // 设置玩家不在范围内
        }
    }
}
