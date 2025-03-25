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
    

    public void ApplyDamage(float Damage)
    {
        Health -= Damage;
    }

}