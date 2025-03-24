using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraCamera : MonoBehaviour
{
    public GameObject Terra;
    private Controller_Terra controller;
    private Transform initialTransform;
    private Vector3 initialPosition;
    private Vector3 positionDifference;
  
    void Start()
    {
        controller = Terra.GetComponent<Controller_Terra>();
        initialTransform = Terra.transform;
        initialPosition = initialTransform.position;
        positionDifference = this.transform.position - initialPosition;

    }


    void Update()
    {
        this.transform.position = Terra.transform.position + positionDifference;
    }
}
