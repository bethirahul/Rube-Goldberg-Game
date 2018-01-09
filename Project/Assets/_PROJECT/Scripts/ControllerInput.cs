using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
	public GameLogic gameLogic;

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
	}
	
	//   U P D A T E                                                                                                    
	public void checkInput()
	{
		if(OVRInput.Get(OVRInput.Button.Two, r_controller))
		{
			///Debug.Log("B button is being pressed");
			r_ray.gameObject.SetActive(true);
			r_ray.SetPosition(0, r_controller_GO.transform.position);

			RaycastHit hit;
			if(Physics.Raycast(r_controller_GO.transform.position,
			                   r_controller_GO.transform.forward, out hit, rayRange, rayMask))
			{
				///float dist = Vector3.Distance(r_controller_GO.transform.position, hit.point);
				///Debug.Log("1. Ray hitting something " + hit.point + "; Distance: " + dist);
				r_ray.SetPosition(1, hit.point);
				if(hit.transform.tag == "Button")
				{
					///Debug.Log("1.1. Its a Button");
					hit.collider.gameObject.GetComponent<VRButton>().hover();
				}
				else
				{
					RaycastHit groundHit;
					if(Physics.Raycast(hit.point, -Vector3.up, out groundHit, rayRange, rayMask))
					{
						///Debug.Log("1.3.1. Ground Ray hitting ground at " + groundHit.point);
						teleportLocation_GO.transform.position = groundHit.point;
						teleportLocation_GO.SetActive(true);
					}
					else
					{
						teleportLocation_GO.SetActive(false);
						Debug.Log("1.3.2. Ground Ray not hitting ground: Cannot Teleport!");
					}
				}
			}
			else
			{
				Vector3 rayEndPoint = (r_controller_GO.transform.forward * rayRange) +
									  r_controller_GO.transform.position;
				///float dist = Vector3.Distance(r_controller_GO.transform.position, rayEndPoint);
				///Debug.Log("2. Ray not hitting anything, end point is at " + rayEndPoint + "; Distance: " + dist);
				r_ray.SetPosition(1, rayEndPoint);

				RaycastHit groundHit;
				if(Physics.Raycast(rayEndPoint, -Vector3.up, out groundHit, rayRange, rayMask))
				{
					///Debug.Log("2.1. Ground Ray hitting ground at " + groundHit.point);
					teleportLocation_GO.transform.position = groundHit.point;
					teleportLocation_GO.SetActive(true);
				}
				else
				{
					teleportLocation_GO.SetActive(false);
					Debug.Log("2.2. Ground Ray not hitting ground: Cannot Teleport!");
				}
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
				///Debug.Log("Ray hit an obstacle");
				if(hit.transform.tag == "Button")
				{
					Debug.Log("Button clicked");
					hit.collider.gameObject.GetComponent<VRButton>().click();
				}
				else
				{
					RaycastHit groundHit;
					if(Physics.Raycast(hit.point, -Vector3.up, out groundHit, rayRange, rayMask))
					{
						///Debug.Log("1.3.1. Ground Ray hitting ground at " + groundHit.point);
						gameLogic.teleport(teleportLocation_GO.transform.position);
					}
					else
					{
						Debug.Log("1.3.2. Ground Ray not hitting ground: Cannot Teleport!");
					}
				}
			}
			else
			{
				Vector3 rayEndPoint = (r_controller_GO.transform.forward * rayRange) +
									  r_controller_GO.transform.position;
				///float dist = Vector3.Distance(r_controller_GO.transform.position, rayEndPoint);
				///Debug.Log("2. Ray not hitting anything, end point is at " + rayEndPoint + "; Distance: " + dist);
				r_ray.SetPosition(1, rayEndPoint);

				RaycastHit groundHit;
				if(Physics.Raycast(rayEndPoint, -Vector3.up, out groundHit, rayRange, rayMask))
				{
					///Debug.Log("2.1. Ground Ray hitting ground at " + groundHit.point);
					gameLogic.teleport(teleportLocation_GO.transform.position);
				}
				else
				{
					Debug.Log("2.2. Ground Ray not hitting ground: Cannot Teleport!");
				}
			}
		}
	}
}
