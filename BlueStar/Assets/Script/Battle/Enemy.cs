using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour,Aircraft
{
    public Orbit orbit;
    public Vector3 center_Star = new Vector2(-1,1);
    public float semiMajorAxis = 5f; // 半长轴
    public float eccentricity = 0.3f; // 偏心率
    public float orbitalSpeed = 0.05f; // 轨道速度（决定飞船在轨道上的速度）
    public float acceleration = 0f; // 外部加速度，决定飞船的加速（正值表示加速，负值表示减速）
    public float Rotation_Tangent=0f;//飞船相对于切线的旋转角度
    public float blood = 4;
    public float harm = 1;
    private float initialSpeed;
    private float initialAcceleration;
    private List<Vector3> points ;
    public int pointCounts = 2;
    private float a;
    private GameObject Inner;//形状这个GameObject
    public EnemyType enemyType;
    
 


    private void OnEnable()
    {
        EventCenter_BattleMode.OnLaunchEmitter += onShoot;

    }
    private void OnDisable()
    {
        EventCenter_BattleMode.OnLaunchEmitter -= onShoot;
    }

    void Start()
    {
        orbit = new Orbit();
        initialSpeed = orbitalSpeed;
        initialAcceleration=acceleration;
  
        orbit.InitializeParameter(this.transform.position,center_Star,orbitalSpeed,acceleration,eccentricity,Rotation_Tangent,semiMajorAxis);
        points = new List<Vector3>();
        Inner=this.transform.Find("Canvas/Inner").gameObject;
        



    }

    void Update()
    {
        Addpoints(orbit.trueAnomaly);
        onMove();

        

    }

    public void onMove()
    { 
        var (newPosition,newCenter, newAcceleration, newTrueAnomaly, newSemiMajorAxis,newOrbitalSpeed) = orbit.UpdatePosition(this.transform.position);
        orbit.UpdateParameter(center_Star,orbitalSpeed,acceleration,eccentricity,Rotation_Tangent,newSemiMajorAxis,newTrueAnomaly);//这个只是给orbit里的方法使用
        //这个物体本身的各个参数还要刷新一遍
        this.transform.position = newPosition;
        a = newSemiMajorAxis;
        
        if (points != null && points.Count >= 2)
        {
            Vector3 tangentFromPoints = (points[1] - points[0]).normalized;
            Inner.transform.up = tangentFromPoints;
        }
    }
    public void onShoot()
    {
        //发射Emitter
        //GameObject.Instantiate();
        //清空发射的ID
        DataManager.updateLaunchedEmitterType = 0;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet1")&& enemyType==EnemyType.threeDimension)
        {
           this.GetComponent<BloodLine>().ReduceBlood();
           blood -= harm;
        }
        else if (other.CompareTag("Bullet2") && enemyType==EnemyType.oneDimension)
        {
            this.GetComponent<BloodLine>().ReduceBlood();
            blood -= harm;
        }
        else if (other.CompareTag("Bullet2") && enemyType==EnemyType.twoDimension)
        {
            this.GetComponent<BloodLine>().ReduceBlood();
            blood -= harm;
        }
        else if (other.CompareTag("Bullet3") && enemyType==EnemyType.fourDimension)
        {
            this.GetComponent<BloodLine>().ReduceBlood();
            blood -= harm;
        }

        if (blood==0)
        {
            Destroy(this.gameObject);
            CleanSignals.totalKill += 1;
        }

        if (other.CompareTag("Froze"))
        {
            this.acceleration = -30f;
            this.orbitalSpeed *= 0.5f;
            
            StartCoroutine(LerpValue(this.acceleration,initialAcceleration,20f));
            StartCoroutine(LerpValue( this.orbitalSpeed,initialSpeed,20f));
            
        }
        
    }
    
    IEnumerator LerpValue(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            from = Mathf.Lerp(from, to, t);
            // 可选：在这里添加需要的更新逻辑，比如 UI 显示、Shader 参数等
            yield return null;
        }
        from = to; // 确保最后精确到目标值
    }
    
    public void Addpoints(float beginAngle)
    {
        
        points.Clear();
        for (int i = 0; i <= pointCounts; i++)
        {
            float deltaAngle = 0.5f * Mathf.PI / pointCounts; //之前这里写的是pointCounts
            float r = a * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(beginAngle));
            float x = r * Mathf.Cos(beginAngle) + center_Star.x;
            float y = r * Mathf.Sin(beginAngle) + center_Star.y;
            Vector3 point = new Vector3(x, y, 0);
            
            beginAngle += deltaAngle;
            points.Add(point);
            a += acceleration * 0.01f;
        }
        
    }
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (points == null) points = new List<Vector3>();
            Addpoints(0f); // 用默认角度画一下（你可以改成其他合理角度）
        }

        if (points == null || points.Count < 2) return;

        Vector3 tangent = (points[1] - points[0]).normalized;
        Vector3 start = points[0];
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, start + tangent * 2f);
    }
    
}

public enum EnemyType
{
    oneDimension,twoDimension,threeDimension,fourDimension
}
