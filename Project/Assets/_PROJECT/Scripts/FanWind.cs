using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanWind : MonoBehaviour
{
	GameObject fan_GO;
	GameLogic GL;

	void Start()
	{
		fan_GO = transform.parent.gameObject;
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
	}

	void OnTriggerStay(Collider collider)
	{
		if(collider.transform.tag == "Throwable")
		{
			
		}
	}
}
