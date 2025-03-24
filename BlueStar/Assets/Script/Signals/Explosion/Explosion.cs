using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator _animator;
    private AnimatorStateInfo info;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        info = _animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
    }
}
