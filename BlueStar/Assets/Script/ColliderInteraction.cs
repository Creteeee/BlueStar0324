using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInteraction : MonoBehaviour
{
    public GameObject interactionUI; 
    public string playerTag = "Player";  

    private void Start()
    {
        // 确保 Interaction UI 初始为隐藏状态
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("请在 Inspector 中分配 Interaction UI 对象！");
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        // 检查是否是 Terra 角色进入
        if (other.gameObject.CompareTag("Player") && interactionUI != null)
        {
            interactionUI.SetActive(true);  
        }
    }

    private void OnCollisionExit(Collision other)
    {
        // 检查是否是 Terra 角色进入
        if (other.gameObject.CompareTag("Player") && interactionUI != null)
        {
            interactionUI.SetActive(false);  
        }
    }
    
}
