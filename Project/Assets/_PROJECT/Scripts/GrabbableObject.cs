using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrabbableObject : MonoBehaviour
{
	public enum objectType
	{
		fan,
		plank,
		belt,
		spring
	};
	public objectType type;

	private GameObject player_GO;
	public Collider[] collider;

	void Start()
	{
		player_GO = GameObject.Find("Player");

		for(int i=0; i<collider.Length; i++)
			Physics.IgnoreCollision(player_GO.GetComponent<Collider>(), collider[i]);
	}
}
