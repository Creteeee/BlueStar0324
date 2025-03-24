using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectUIManager : MonoBehaviour
{
    public SpaceShip spaceship;
    public Emitter currentEmitter;
    public int currentEmitterNumber = 0;
    
    
    
    //public GameObject currentEmitterModel;

   /* void Start()
    {
        if (spaceship.instantiatedEmitters.Count > 0)
        {
            currentEmitter = spaceship.instantiatedEmitters[currentEmitterNumber].GetComponent<Emitter>();
        }
    }

    void Update()
    {
        if (spaceship.instantiatedEmitters.Count > currentEmitterNumber)
        {
            currentEmitter = spaceship.instantiatedEmitters[currentEmitterNumber].GetComponent<Emitter>();
        }

        //currentEmitterModel = spaceship.currentEmitterModel;
    }

    public void onClickLaunch()
    {
        spaceship.isLaunch = true;
    }

    public void onClickLeftButton()
    {
        if (currentEmitter != null)
            currentEmitter.RotateSign = 1;
    }

    public void onClickRightButton()
    {
        if (currentEmitter != null)
            currentEmitter.RotateSign = -1;
    }

    public void onClickUpButton()
    {
        if (currentEmitter != null)
            currentEmitter.AccelerationSign = 1;
    }

    public void onClickDownButton()
    {
        if (currentEmitter != null)
            currentEmitter.AccelerationSign = -1;
    }

    public void onClickChangeEmitterRight()
    {
        if (currentEmitterNumber < spaceship.instantiatedEmitters.Count - 1)
        {
            currentEmitterNumber++;
        }
        else
        {
            currentEmitterNumber = 0;
        }
        Debug.Log("");
        Debug.Log($"当前的emitter序号为{currentEmitterNumber}");
    }

    public void onClickChangeEmitterLeft()
    {
        if (currentEmitterNumber > 0)
        {
            currentEmitterNumber--;
        }
        else
        {
            currentEmitterNumber = spaceship.instantiatedEmitters.Count - 1;
        }
    }*/
}