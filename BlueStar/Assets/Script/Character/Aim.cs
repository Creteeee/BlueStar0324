using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Aim : MonoBehaviour
{
    private GameObject linePrefab;
    private GameObject line;
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private Vector3 hitPoint;
    public GameObject Marker;
    public Transform startPoint;
    public Transform endPoint;
    private Vector3 rayDir;
    public GameObject bullet;
    private GameObject bulletInst;
    public GameObject light;
    public Transform bulletPoint;
    public GameObject bloodVFX;
    private GameObject bloodVFXInst;

    private void Awake()
    {
        linePrefab = Resources.Load<GameObject>("Prefabs/Line/Line_1");
        //bloodVFX = Resources.Load<GameObject>("Prefabs/VFX/Blood_Spray");
        bullet=Resources.Load<GameObject>("Prefabs/Bullet_Weapon/Bullet_Pistol");
        line = Instantiate(linePrefab,this.transform);
        line.transform.localScale = new Vector3(50, 50, 50);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        points.Add(new Vector3(0, 0, 0));
        points.Add(new Vector3(0, 0, 0));
        Marker.SetActive(false);
        light = this.transform.GetChild(0).gameObject; 

    }

    private void OnDisable()
    {
        Marker.SetActive(false);

    }

    void Update()
    {
        // 计算从 startPoint 到 endPoint 的射线方向（相对位置）
        rayDir = (endPoint.position - startPoint.position).normalized;
        

        // 创建射线
        Ray ray = new Ray(startPoint.position, rayDir);
        RaycastHit hit;

        // 射线检测
        if (Physics.Raycast(ray, out hit, 400f))
        {
            if (hit.collider.gameObject.CompareTag("Zombie"))
            {
                Debug.Log("射线击中了物体: " + hit.collider.gameObject.name);
                hitPoint = hit.point;
                Marker.SetActive(true);
                Marker.transform.position = hitPoint+new Vector3(0,0.5f,-0.5f); // 设置标记物体位置
                
            }
        }
        else
        {
            Marker.SetActive(false);

        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            
            bulletInst=Instantiate(bullet,bulletPoint.transform.position,Quaternion.identity);
            // bloodVFXInst = Instantiate(bloodVFX, hitPoint, Quaternion.identity);
            // bloodVFXInst.transform.rotation=Quaternion.Euler(new Vector3(30f,0,0));
            // bloodVFXInst.transform.localScale *= 1f;
            // Destroy(bloodVFXInst ,1f);
            Quaternion targetRotation = Quaternion.FromToRotation(bulletInst.transform.forward, this.rayDir);
            bulletInst.transform.rotation = targetRotation;
            light.GetComponent<Light>().intensity = 20;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            light.GetComponent<Light>().intensity = 0;

        }
        


        // 更新LineRenderer的位置，显示射线
        points[0] = startPoint.position; 
        points[1] = ray.origin + rayDir * 2f;
        lineRenderer.SetPositions(points.ToArray());
    }
}
