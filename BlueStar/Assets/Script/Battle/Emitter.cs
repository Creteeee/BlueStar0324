
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Emitter : MonoBehaviour,Aircraft
{

    public Orbit orbit;
    public Vector3 center_Star = new Vector2(-1,1);
    public float semiMajorAxis = 5f; // 半长轴
    public float eccentricity = 0.3f; // 偏心率
    public float orbitalSpeed = 0.8f; // 轨道速度（决定飞船在轨道上的速度）
    public float acceleration = 0f; // 外部加速度，决定飞船的加速（正值表示加速，负值表示减速）
    public float Rotation_Tangent=0f;//飞船相对于切线的旋转角度
    private SpaceShip sp;
    private List<Vector3> points ;
    public int pointCounts = 50;
    private GameObject linePrefab;
    private GameObject line;
    private LineRenderer lineRenderer;
    private int timer=0;
    private float a;
    public Vector3 CameraDir;
    public GameObject Camera;
    public List<GameObject> enemies;
    public float minDistance = 1f;
    public GameObject bulletPrefab;
    public Vector3 TangentDir=new Vector3();
    public static Vector3 BulletDir;
    public GameObject enemy;
    public static int currentBulletID = -1;
    
    

    
        void Start()
    {
        orbit = new Orbit();
        sp =GameObject.Find("SpaceShip").GetComponent<SpaceShip>();
        center_Star = sp.center_Star;
        semiMajorAxis = sp.semiMajorAxis+0.01f;
        orbitalSpeed = sp.orbitalSpeed + 0.01f;
        orbit.InitializeParameter(this.transform.position,center_Star,sp.orbitalSpeed,acceleration,sp.eccentricity,Rotation_Tangent,semiMajorAxis);
        points = new List<Vector3>();
        linePrefab = Resources.Load<GameObject>("Prefabs/Line/Line");
        bulletPrefab=Resources.Load<GameObject>("Prefabs/Bullet/Type_1/Bullet_Type1");
        a = semiMajorAxis;
        Camera.SetActive(true);
        string nameToSearch = "Enemy";  // 要匹配的名称部分
        // 获取场景中所有的游戏物体
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // 如果物体的名称包含给定的部分
            if (obj.name.Contains(nameToSearch))
            {
                enemies.Add(obj);
            }

            enemy = GameObject.Find("Enemy_Type1");
        }
        

    }

    private void OnEnable()
    {
     
    }

    void Update()
    {
        if (timer < 1)
        {
            line=Instantiate(linePrefab);
            timer++;
        }
        onMove();
        Addpoints(orbit.trueAnomaly);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCounts;
        lineRenderer.SetPositions(points.ToArray());
        Debug.Log("deltatime是"+1/Time.deltaTime);
        
        CameraDir = orbit.CameraDir;
        Camera.transform.LookAt(enemy.transform.position);
        
        foreach (GameObject i in enemies)
        {
            float distance = (this.transform.position - i.transform.position).magnitude;
            if (distance < minDistance)
            {
                Camera.SetActive(true);
            }
            else
            {
                Camera.SetActive(false);
            }
        }
        /*AlignCameraWithOrbit();*/
        if (Input.GetKeyDown(KeyCode.Mouse0) && Camera.activeSelf==true)
        {
            onShoot();
        }


    }

    public void onMove()
    {
        acceleration = DataManager.emitterAcceleration;
        var (newPosition,newCenter, newAcceleration, newTrueAnomaly, newSemiMajorAxis,newOrbitalSpeed) = orbit.UpdatePosition(this.transform.position);
        orbit.UpdateParameter(center_Star,orbitalSpeed,acceleration,eccentricity,Rotation_Tangent,newSemiMajorAxis,newTrueAnomaly);//这个只是给orbit里的方法使用
        //这个物体本身的各个参数还要刷新一遍
        a = newSemiMajorAxis;
        this.transform.position = newPosition;
    }
    public void onShoot()
    {
        
      Vector2  mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, UnityEngine.Camera.main.farClipPlane));
      if (Input.mousePosition.x < Screen.width * 0.5 && Input.mousePosition.y < Screen.height && DataManager.bulletCounts>0)
      {
          GameObject bullet =GameObject.Instantiate(bulletPrefab,this.transform.position,quaternion.identity);
          DataManager.bulletCounts -= 1;
      }
      Debug.Log("mousepos是" +mousePos);
      BulletDir =Vector3.Normalize(new Vector3(mousePos.x, mousePos.y, 1));
      currentBulletID += 1;
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
    // 计算相机的位置和朝向
    /*private void AlignCameraWithOrbit()
    {
        //Vector3 position = orbit.GetPosition();
        Vector3 velocity = orbit.GetVelocity();

        // 计算切线方向（即速度方向）
        Vector3 tangentDirection = velocity.normalized;
        TangentDir = tangentDirection;

        // 计算相机应该朝向的切线方向
        Quaternion rotation = Quaternion.LookRotation(tangentDirection, Vector3.up);

        // 将相机绕世界的 Z 轴旋转 90 度
        Quaternion localRotation = rotation * Quaternion.Euler(0f, 90f, 0f); // 绕 X 轴旋转 90 度

        // 设置相机的旋转
        Camera.transform.rotation = localRotation;
    }*/


}
