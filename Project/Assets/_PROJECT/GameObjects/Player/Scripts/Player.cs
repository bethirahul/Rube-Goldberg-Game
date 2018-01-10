using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private bool isMoving;
	private Vector3 teleportLocation;
	public float teleportSpeed;
	private Vector3 startPosition;
	
	//   S T A R T                                                                                                      
	public void Init()
	{
		isMoving = false;
	}

	public void InitTeleport(Vector3 tLoc)
	{
		teleportLocation = tLoc;
		startPosition = gameObject.transform.position;
		isMoving = true;
	}

	//   U P D A T E                                                                                                    
	public void Teleport()
	{
		if(isMoving)
		{
			float dist = Vector3.Distance(startPosition, gameObject.transform.position);
			if(dist <= teleportSpeed)
			{
				gameObject.transform.position = teleportLocation;
				isMoving = false;
			}
			else
			{
				gameObject.transform.position = teleportLocation;
			}
		}
	}
}
