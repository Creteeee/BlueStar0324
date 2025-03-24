using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceShip : MonoBehaviour,Aircraft
{
    public Orbit orbit;
    public Vector3 center_Star = new Vector2(-1,1);
    public float semiMajorAxis = 5f; // 半长轴
    public float eccentricity = 0.3f; // 偏心率
    public float orbitalSpeed = 0.05f; // 轨道速度（决定飞船在轨道上的速度）
    public float acceleration = 0f; // 外部加速度，决定飞船的加速（正值表示加速，负值表示减速）
    public float Rotation_Tangent=0f;//飞船相对于切线的旋转角度
    private List<Vector3> points ;
    public int pointCounts = 50;
    private GameObject linePrefab;
    private GameObject line;
    private LineRenderer lineRenderer;
    public List<GameObject> EmitterList { get;  set; } = new List<GameObject>();
    public GameObject spaceshipModel;
    public float anglebbias = -25f;

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
        points = new List<Vector3>();
        orbit = new Orbit();
        orbit.InitializeParameter(this.transform.position,center_Star,orbitalSpeed,acceleration,eccentricity,Rotation_Tangent,semiMajorAxis);
        linePrefab = Resources.Load<GameObject>("Prefabs/Line/Line");
        line=Instantiate(linePrefab);
        spaceshipModel = transform.Find("Spaceship1").gameObject;
        this.transform.up =new Vector3(Mathf.Cos(orbit.trueAnomaly),Mathf.Sin(orbit.trueAnomaly),0);


    }

    void Update()
    {
        onMove();
        Addpoints(orbit.trueAnomaly);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCounts;
        lineRenderer.SetPositions(points.ToArray());
        //Debug.Log(orbit.GetTangentDirection());
        
        
        // 计算切线方向
        Vector3 tangentDirection = orbit.GetTangentDirection();

        // 设置飞船的朝向
        float angle = Mathf.Atan2(tangentDirection.y*eccentricity, tangentDirection.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, 0, angle+anglebbias);
       // this.transform.rotation= Vector3(Mathf.Cos(orbit.trueAnomaly),Mathf.Sin(orbit.trueAnomaly),0);


    }

    public void onMove()
    { 
        var (newPosition,newCenter, newAcceleration, newTrueAnomaly, newSemiMajorAxis,newOrbitalSpeed) = orbit.UpdatePosition(this.transform.position);
        orbit.UpdateParameter(center_Star,orbitalSpeed,acceleration,eccentricity,Rotation_Tangent,newSemiMajorAxis,newTrueAnomaly);//这个只是给orbit里的方法使用
        //这个物体本身的各个参数还要刷新一遍
        this.transform.position = newPosition;
    }
    public void onShoot()
    {
        //发射Emitter
        //GameObject.Instantiate(DataManager.emitterConfigs[DataManager.updateLaunchedEmitterType].Prefab,this.transform.position,quaternion.identity);
        GameObject.Instantiate(DataManager.emitterConfigs[1].Prefab,this.transform.position,quaternion.identity);
        //清空发射的ID
        DataManager.updateLaunchedEmitterType = 0;

    }

    public void Addpoints(float beginAngle)
    {
        points.Clear();
        for (int i = 0; i <= pointCounts; i++)
        {
            float deltaAngle =  2 * Mathf.PI / pointCounts ;//之前这里写的是pointCounts
            float r = semiMajorAxis * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(beginAngle));
            float x = r * Mathf.Cos(beginAngle)+center_Star.x;
            float y = r * Mathf.Sin(beginAngle)+center_Star.y;
            Vector3 point = new Vector3(x, y, 0) ;
            beginAngle += deltaAngle;
            points.Add(point);
        }
        
    }

}
