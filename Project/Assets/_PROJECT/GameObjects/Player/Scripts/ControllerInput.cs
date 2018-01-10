using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
	public GameLogic gameLogic;
	public Player player;

	//  OCULUS
	///public OVRInput.Controller l_controller;
	public OVRInput.Controller r_controller;
	///public GameObject l_controller_GO;
	public GameObject r_controller_GO;

	//  RAY CASTING
	///public LineRenderer l_ray;
	public LineRenderer r_ray;
	public LayerMask rayMask;
	public float rayRange;

	//  HOLDING
	private enum Action
	{
		none,
		holding,
		rayCasting,
		menu
	};
	private Action action;

	//  TELEPORT
	public GameObject teleportLocation_GO;

	//   S T A R T                                                                                                      
	void Start()
	{
		///l_ray.gameObject.SetActive(false);
		r_ray.gameObject.SetActive(false);
		player = GetComponent<Player>();
	}
	
	//   U P D A T E                                                                                                    
	public void CheckInput()
	{
		if(OVRInput.Get(OVRInput.Button.Two, r_controller))
		{
			r_ray.gameObject.SetActive(true);
			r_ray.SetPosition(0, r_controller_GO.transform.position);

			RaycastHit hit;
			if(Physics.Raycast(r_controller_GO.transform.position,
			                   r_controller_GO.transform.forward, out hit, rayRange, rayMask))
			{
				r_ray.SetPosition(1, hit.point);

				if(hit.transform.tag == "Button")
					hit.collider.gameObject.GetComponent<VRButton>().hover();
				else
				if(hit.transform.tag == "Ground")
				{
					teleportLocation_GO.transform.position = hit.point;
					teleportLocation_GO.SetActive(true);
				}
				else
					GroundRay(hit.point, true);
			}
			else
			{
				Vector3 rayEndPoint = (r_controller_GO.transform.forward * rayRange) +
				                      r_controller_GO.transform.position;
				r_ray.SetPosition(1, rayEndPoint);

				GroundRay(rayEndPoint, true);
			}
		}

		if(OVRInput.GetUp(OVRInput.Button.Two, r_controller))
		{
			Debug.Log("B button released");
			r_ray.gameObject.SetActive(false);
			teleportLocation_GO.SetActive(false);

			RaycastHit hit;
			if(Physics.Raycast(r_controller_GO.transform.position,
			                   r_controller_GO.transform.forward, out hit, rayRange, rayMask))
			{
				if(hit.transform.tag == "Button")
					hit.collider.gameObject.GetComponent<VRButton>().click();
				else
				if(hit.transform.tag == "Ground")
				{
					gameLogic.InitTeleportPlayer(hit.point);
				}
				else
					GroundRay(hit.point, false);
			}
			else
			{
				Vector3 rayEndPoint = (r_controller_GO.transform.forward * rayRange) +
				                      r_controller_GO.transform.position;
				r_ray.SetPosition(1, rayEndPoint);

				GroundRay(rayEndPoint, false);
			}
		}

		Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, r_controller);
		if(joystickInput != Vector2.zero)
			player.Move(joystickInput);
		/*else
			player.StopMoving();*/
	}

	private void GroundRay(Vector3 startPoint, bool isButtonPress)
	{
		RaycastHit groundHit;
		if(Physics.Raycast(startPoint, -Vector3.up, out groundHit, rayRange, rayMask))
		{
			if(isButtonPress)
			{
				teleportLocation_GO.transform.position = groundHit.point;
				teleportLocation_GO.SetActive(true);
			}
			else
				gameLogic.InitTeleportPlayer(groundHit.point);
		}
		else
		{
			if(isButtonPress)
				teleportLocation_GO.SetActive(false);
			Debug.Log("Ground Ray didn't hit ground: Cannot Teleport!");
			Debug.Log("First Ray end: " + startPoint);
		}
	}
}
