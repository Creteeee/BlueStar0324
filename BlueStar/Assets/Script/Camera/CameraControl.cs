using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Net;
using System.IO;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    public float MoveSpeed = 2;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        this.transform.position += new Vector3(x, y, 0);
    }

}
