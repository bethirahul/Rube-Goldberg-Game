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

	/*private Collider player_collider;
	public Collider[] collider;

	void Start()
	{
		player_collider = GameObject.Find("Player").GetComponent<Collider>();

		for(int i=0; i<collider.Length; i++)
			Physics.IgnoreCollision(player_collider, collider[i]);
	}*/
}
