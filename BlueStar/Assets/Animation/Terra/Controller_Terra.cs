using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum StateType
{
    // Idle_Neutral,
    IdleWalkRun,
    Walking,
    Running,
    Aiming,
    Shooting,
}

public class Controller_Terra : MonoBehaviour
{
    private StateType curState;
    private Animator animator;
    public float walkThreshold = 0.1f; // 输入的阈值，避免轻微输入就触发行走
    public float walkSpeed = 2.0f; // 行走速度
    private float walkSpeedCurrent=0f;
    public float runSpeedMultiplier = 2.0f; // 跑步速度倍增
    public float rotationSpeed = 10.0f; // 转向速度
    public GameObject gun;
    public DialogueManager dialogManager;
    private Dictionary<string, int> posDic = new Dictionary<string, int>();
    private int dialog_fish = 0;
    private bool isdialog_Fish = false;
    private int dialog_cat = 0;
    private bool isdialog_cat = false;
    private int dialog_entrance = 0;
    private bool isdialog_entrance = false;
    private int dialog_window = 0;
    private bool isdialog_window = false;
    private int dialog_notice = 0;
    private bool isdialog_notice = false;
    private int dialog_gun = 0;
    private bool isdialog_gun = false;
    private GameObject shootLine;
    private GameObject shortLineInst;
    public List<Vector3> shootLinePoints=new List<Vector3>();
    private bool isFading = true;
    public float Health = 100.0f;


    private void Awake()
    {
        shootLine = Resources.Load<GameObject>("Prefabs/Line/Line");
        if (shootLine != null)
        {
            Debug.Log("成功加载射击显示线,名字为："+shootLine.name);
        }
    }

    void Start()
    {
        curState = StateType.IdleWalkRun;
        animator = GetComponent<Animator>();
        gun.SetActive(false);
        posDic ["fishTank"] = 0;
        posDic ["cat"] = 1;
        posDic["entrance"] = 2;
        posDic["window"] = 3;
        posDic["notice"] = 4;
        shortLineInst = Instantiate(shootLine, this.transform);
        animator.SetFloat("Blend", 0);
        walkSpeedCurrent = 0;
    }

    void Update()
    {
        // 获取当前的水平和垂直输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(horizontal, vertical);

        // 处理瞄准和射击状态
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (curState != StateType.Shooting)
            {
                curState = StateType.Aiming;
                onAiming();
            }

            // 按下鼠标左键进入射击状态
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                curState = StateType.Shooting;
                onShooting();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
          
            // 松开右键时进入 IdleWalkRun 状态
            curState = StateType.IdleWalkRun;
            walkSpeedCurrent = 0;  // 确保速度为0
            onWalking(inputVector);  // 停止行走动画
        }
        else
        {
            // 没有右键按下时，检查角色输入并切换状态
            if (inputVector.magnitude >=0)
            {
                curState = StateType.IdleWalkRun;  // 切换到行走状态
                onWalking(inputVector);  // 更新行走动画
            }
            else
            {
                curState = StateType.IdleWalkRun;
                onWalking(inputVector); // 没有输入时，保持静止
                //walkSpeedCurrent = 0f;  // 确保停止移动

            }
        }
        animator.SetFloat("Blend", walkSpeedCurrent);
        transform.Translate(Vector3.forward * walkSpeedCurrent * Time.deltaTime);

        // 在 Aiming 状态下实时跟随鼠标位置旋转角色
        if (curState == StateType.Aiming)
        {
            RotateTowardsMouse();
        }

        // if (isdialog_Fish == true && Input.GetKeyDown(KeyCode.E))
        // {
        //     dialogManager.Awake();
        //     dialogManager.Start();
        //     UI.GetComponent<CanvasGroup>().interactable = true;
        //     UI.GetComponent<CanvasGroup>().alpha = 1;
        //     focusCamera.gameObject.SetActive(true);
        //     focusCamera.gameObject.transform.position = focusPoses[posDic["fishTank"]].position;
        //     focusCamera.gameObject.transform.rotation = focusPoses[posDic["fishTank"]].rotation;
        // }
        // else if (isdialog_cat == true && Input.GetKeyDown(KeyCode.E))
        // {
        //     dialogManager.Awake();
        //     dialogManager.Start();
        //     UI.GetComponent<CanvasGroup>().interactable = true;
        //     UI.GetComponent<CanvasGroup>().alpha = 1;
        //     focusCamera.gameObject.SetActive(true);
        //     focusCamera.gameObject.transform.position = focusPoses[posDic["cat"]].position;
        //     focusCamera.gameObject.transform.rotation = focusPoses[posDic["cat"]].rotation;
        //     
        // }
        // else if (isdialog_entrance == true && Input.GetKeyDown(KeyCode.E))
        // {
        //     dialogManager.Awake();
        //     dialogManager.Start();
        //     UI.GetComponent<CanvasGroup>().interactable = true;
        //     UI.GetComponent<CanvasGroup>().alpha = 1;
        //     focusCamera.gameObject.SetActive(true);
        //     focusCamera.gameObject.transform.position = focusPoses[posDic["entrance"]].position;
        //     focusCamera.gameObject.transform.rotation = focusPoses[posDic["entrance"]].rotation;
        // }
        // else if (isdialog_window == true && Input.GetKeyDown(KeyCode.E))
        // {
        //     dialogManager.Awake();
        //     dialogManager.Start();
        //     UI.GetComponent<CanvasGroup>().interactable = true;
        //     UI.GetComponent<CanvasGroup>().alpha = 1;
        //     focusCamera.gameObject.SetActive(true);
        //     focusCamera.gameObject.transform.position = focusPoses[posDic["window"]].position;
        //     focusCamera.gameObject.transform.rotation = focusPoses[posDic["window"]].rotation;
        // }
        //
        // else if (isdialog_notice == true && Input.GetKeyDown(KeyCode.E))
        // {
        //     dialogManager.Awake();
        //     dialogManager.Start();
        //     UI.GetComponent<CanvasGroup>().interactable = true;
        //     UI.GetComponent<CanvasGroup>().alpha = 1;
        //     focusCamera.gameObject.SetActive(true);
        //     focusCamera.gameObject.transform.position = focusPoses[posDic["notice"]].position;
        //     focusCamera.gameObject.transform.rotation = focusPoses[posDic["notice"]].rotation;
        // }
        // else if (isdialog_gun == true && Input.GetKeyDown(KeyCode.E))
        // {
        //     dialogManager.Awake();
        //     dialogManager.Start();
        //     UI.GetComponent<CanvasGroup>().interactable = true;
        //     UI.GetComponent<CanvasGroup>().alpha = 1;
        //
        // }
        //
        //
        // if (dialogManager.dialogIndex == 36)
        // {
        //     UI_MR.SetActive(true);
        // }
        // if (dialogManager.dialogIndex == 57)
        // {
        //     SceneManager.LoadScene(4);
        // }
        
        
    }

    void onWalking(Vector2 inputVector)
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isAiming", false);
        gun.SetActive(false);
       

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = targetRotation;  // 直接设置目标旋转
            if (inputVector.x >= walkThreshold || inputVector.y >= walkThreshold)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (walkSpeedCurrent < walkSpeed*runSpeedMultiplier)
                    {
                        walkSpeedCurrent += 0.01f;  // 奔跑状态 
                    }
                    else
                    {
                        walkSpeedCurrent = walkSpeed*runSpeedMultiplier;
                    }
                    
                }
                else
                {
                    if (walkSpeedCurrent > walkSpeed)
                    {
                        walkSpeedCurrent -= 0.01f;  // 行走状态
                    }
                    else
                    {
                        walkSpeedCurrent = walkSpeed; 
                    }

                }
            }
            else
            {
                if (walkSpeedCurrent > walkSpeed)
                {
                    walkSpeedCurrent -= 0.001f;  // 行走状态
                }
                
            }
        }
        else
        {
            if (walkSpeedCurrent>0)
            {
                walkSpeedCurrent -= 0.02f;
                walkSpeedCurrent=Mathf.Clamp(walkSpeedCurrent, 0, walkSpeed);
            }

        }
      
        
        animator.SetFloat("Blend", walkSpeedCurrent);
        // 移动角色
        
    }

    void onRunning(Vector2 inputVector)
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        gun.SetActive(false);

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = targetRotation;  // 直接设置目标旋转
        }

        transform.Translate(Vector3.forward * walkSpeed * runSpeedMultiplier * Time.deltaTime*10);
    }

    void onAiming()
    {
        animator.SetBool("isAiming", true);
        animator.SetBool("isShooting", false);
        walkSpeedCurrent = 0f;
        gun.SetActive(true);
    }

    void onShooting()
    {
        animator.SetBool("isShooting", true);
        animator.SetBool("isAiming", true);
        gun.SetActive(true);

        StartCoroutine(ResetShooting());
    }

    IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isShooting", false);
    }

    // 角色根据 WASD 输入方向旋转
    void RotateWithInput(float horizontal, float vertical)
    {
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = targetRotation;  // 直接设置目标旋转
        }
    }

    // 角色根据鼠标位置旋转
    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dist1 = -Camera.main.transform.position+ this.transform.position;
        Vector3 pos = (dist1.y - ray.direction.y) / ray.direction.y * ray.direction + Camera.main.transform.position;//+this.GetComponent<Collider>().bounds.size.y/2*Vector3.up;
        
        Debug.DrawLine(transform.position, pos, Color.yellow);
        Debug.DrawLine(transform.position,transform.forward + transform.position,Color.yellow);
        Vector3 direction = pos - transform.position;
        direction.y = 0; // 保持水平旋转

        if (direction != Vector3.zero)
        {
            float angle = Vector3.Angle(Vector3.forward, direction);
            float angle2 = 360-Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            transform.rotation =Quaternion.Euler(0,angle2+180,0);  // 直接设置目标旋转
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case"fishTank" :
                Debug.Log("我碰到鱼缸了");
                if (dialog_fish>=0)
                {
                    DialogueManager.currentDialogueBeginID = 14;
                    dialog_fish++;
                    isdialog_Fish = true;
                    Debug.Log("可以开启鱼缸对话");

                }
                break;
            case"cat" :
                Debug.Log("我碰到大毛了");
                if (dialog_cat >=0)
                {
                    DialogueManager.currentDialogueBeginID = 27;
                    dialog_cat++;
                    isdialog_cat = true;
                }

                break;
            case"entrance" :
                Debug.Log("我碰到大门了");
                if (dialog_entrance >=0)
                {
                    DialogueManager.currentDialogueBeginID = 49;
                    dialog_entrance++;
                    isdialog_entrance = true;

                }

                break;
            case"window" :
                Debug.Log("我碰到窗户了");
                if (dialog_window >=0)
                {
                    DialogueManager.currentDialogueBeginID = 63;
                    dialog_window++;
                    isdialog_window = true;

                }
                break;
            case"notice" :
                Debug.Log("我碰到公告了");
                if (dialog_notice >=0)
                {
                    DialogueManager.currentDialogueBeginID = 81;
                    dialog_notice++;
                    isdialog_notice = true;

                }
                break;
            case"pistol" :
                Debug.Log("我碰到枪了");
                if (dialog_gun <1)
                {
                    DialogueManager.currentDialogueBeginID = 114;
                    dialog_gun++;
                    isdialog_gun = true;

                }
                break;
                
        }
    }

    private void OnCollisionExit(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case"fishTank" :
            {
                isdialog_Fish = false;
            } 
                break;
            case"cat" :
            {
                isdialog_cat = false;
            }

                break;
            case"entrance" :

            {
                isdialog_entrance = false;
            }
                break;
            case "window":
            {
                isdialog_window = false;
            }
                break;
            case "notice":
            {
                isdialog_notice = false;
            }
                break;
            case "gun":
            {
                isdialog_gun = false;
            }
                break;
        }
    }

    public void ApplyDamage(float Damage)
    {
        Health -= Damage;
    }

}