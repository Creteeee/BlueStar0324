using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;



public class IDCardTrigger : MonoBehaviour
{
    public GameObject interactionUI; 
    public string playerTag = "Player";
    public GameObject button1;

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

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是 Terra 角色进入
        if (other.CompareTag(playerTag) && interactionUI != null)
        {
            interactionUI.SetActive(true);  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 检查是否是 Terra 角色离开
        if (other.CompareTag(playerTag) && interactionUI != null)
        {
            interactionUI.SetActive(false);  
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

    public void ButtonClickEffect()
    {
        //button1.transform.DOMove(new Vector2(transform.position.x , transform.position.y-10), 0.5f).From();
        button1.transform.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.3f).From();
    }
    
}