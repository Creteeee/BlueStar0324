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
	[SerializeField] private GameObject suggsetIcon;
	
	

	private void Start()
	{
		initialPosition = this.transform.position;
		right =this.transform.right;
		audio = this.GetComponent<AudioSource>();
		teleport = this.GetComponent<Teleport>();
		if (suggsetIcon!=null)
		{
			suggsetIcon.SetActive(false);
		}

		if (InventoryStateManager.Instance.DoorStates.TryGetValue(this.name,out bool openstate))
		{
			canOpen = openstate;
			if (this.GetComponent<DetectPlayerItem>() != null && canOpen)
			{
				Destroy(this.GetComponent<DetectPlayerItem>());
			}
		}
		else
		{
			InventoryStateManager.Instance.DoorStates.Add(this.name,canOpen);
		}
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

	private void OnDisable()
	{
		Debug.Log("门执行了OnDisable");
		if (!InventoryStateManager.Instance.DoorStates.TryGetValue(this.name,out bool openstate))
		{
			Debug.Log("字典里不存在这个物体的键值对");
		}
		InventoryStateManager.Instance.SaveDoorState(this.name,canOpen);
		Debug.Log(this.gameObject.name+canOpen);
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
			if (suggsetIcon!=null)
			{
				suggsetIcon.SetActive(true);
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
			
			if (suggsetIcon!=null)
			{
				suggsetIcon.SetActive(false);
			}
		}


	}
}
