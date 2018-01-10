using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private bool isMoving;
	private Vector3 teleportLocation;
	public float teleportSpeed;
	private Vector3 startPosition;
	private float lerpDistance;
	private float totalDistance;
	
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
		lerpDistance = 0;
		totalDistance = Vector3.Distance(startPosition, teleportLocation);
	}

	//   U P D A T E                                                                                                    
	public void Teleport()
	{
		if(isMoving)
		{
			///gameObject.transform.position = teleportLocation;
			lerpDistance = lerpDistance + ((Time.deltaTime * teleportSpeed) / totalDistance);
			gameObject.transform.position = Vector3.Lerp(startPosition, teleportLocation, lerpDistance);
			if(lerpDistance >= 1)
			{
				gameObject.transform.position = teleportLocation;
				isMoving = false;
			}
		}
	}
}
