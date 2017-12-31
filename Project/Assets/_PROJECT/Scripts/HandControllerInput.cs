using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

public class HandControllerInput : MonoBehaviour
{
	//  CONTROLLER   
	public SteamVR_TrackedObject trackedObj;
	public SteamVR_Controller.Device device;

	//  TELEPORTER   
	private LineRenderer laser;
	public GameObject teleporterAimerObject;
	public GameObject player;
	public LayerMask laserMask;
	public Vector3 teleportLocation;
	public float yNudgeAmount = 1f; /// to teleport a little over the floor/ground
	public float teleportRadius = 15f;

	//   S T A R T                                                                                                      
	void Start ()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		laser = GetComponentInChildren<LineRenderer>();
	}
	
	//   U P D A T E                                                                                                    
	void Update()
	{
		device = SteamVR_Controller.Input((int)trackedObj.index);

		if(device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
		{
			laser.gameObject.SetActive(true);
			teleporterAimerObject.SetActive(true);

			laser.SetPosition(0, gameObject.transform.position);
			RaycastHit hit;

			if(Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask))
			{
				teleportLocation = hit.point;
				laser.SetPosition(1, teleportLocation);

				// Aimer Position
				teleporterAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
			}
			else
			{
				teleportLocation = (transform.forward * 15) + transform.position;
				RaycastHit groundRay;
				if(Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
				{
					teleportLocation.y = groundRay.point.y;
				}
				laser.SetPosition(1, (transform.forward * 15) + transform.position);

				// Aimer Position
				teleporterAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
			}
		}
		if(device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
		{
			laser.gameObject.SetActive(false);
			teleporterAimerObject.SetActive(false);
		}
		///if(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
	}
}
