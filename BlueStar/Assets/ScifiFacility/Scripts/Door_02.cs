using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class Door_02 : MonoBehaviour
{

	private Vector3 initialPosition;
	private new Vector3 right;
	private AudioSource audio;
	public bool canOpen = true; //道具是否使用
	private int timer = 0;//第一次开门
	private Teleport teleport;
	private bool isTrigger=false;
	
	

	private void Start()
	{
		initialPosition = this.transform.position;
		right =this.transform.right;
		audio = this.GetComponent<AudioSource>();
		teleport = this.GetComponent<Teleport>();
	}

	private void Update()
	{
		if (this.GetComponent<DetectPlayerItem>()!=null)
		{
			canOpen = this.GetComponent<DetectPlayerItem>().isFinished;
		}
		else
		{
			canOpen = true;
		}

		if (timer==0 && canOpen && isTrigger)
		{
			audio.Play();
			this.transform.DOMove(initialPosition+right*1.5f, 1f).OnComplete(() =>
			{
				teleport.onTransitionToScene(); // 只在门完全打开后切换场景
			});
			isTrigger=false;
			timer=1;
		}
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isTrigger = true;
			if (canOpen && timer !=0)
			{
				audio.Play();
				this.transform.DOMove(initialPosition+right*1.5f, 1f).OnComplete(() =>
				{
					teleport.onTransitionToScene(); // 只在门完全打开后切换场景
				});
    				
			}

			
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isTrigger = false;
			if (canOpen)
			{
				if (other.CompareTag("Player"))
				{
					this.transform.DOMove(initialPosition, 1f);
				
				}			
			}
		}


	}
}
