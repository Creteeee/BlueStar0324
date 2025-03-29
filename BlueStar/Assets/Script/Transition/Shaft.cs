using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaft : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.GetComponent<Teleport>()!=null)
            {
                this.GetComponent<Teleport>().onTransitionToScene();
            }
        }
    }
}
