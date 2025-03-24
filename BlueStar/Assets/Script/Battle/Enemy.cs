using System;
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
  
        orbit.InitializeParameter(this.transform.position,center_Star,orbitalSpeed,acceleration,eccentricity,Rotation_Tangent,semiMajorAxis);


    }

    void Update()
    {
        onMove();

        

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
        //GameObject.Instantiate();
        //清空发射的ID
        DataManager.updateLaunchedEmitterType = 0;

    }

}
