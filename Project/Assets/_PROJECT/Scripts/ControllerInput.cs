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
	public LayerMask rayRange;
	public float rayRadius;

	//  HOLDING
	private enum Action
	{
		none,
		holding,
		rayCasting,
		menu
	};
	private Action action;

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
			                   r_controller_GO.transform.forward, out hit, rayRadius))///, rayRange))
			{
				Debug.Log("Ray hitting obstacle");
				r_ray.SetPosition(1, hit.point);
				if(hit.transform.tag == "Button")
				{
					Debug.Log("Its a Button");
					hit.collider.gameObject.GetComponent<VRButton>().hover();
				}
			}
			else
			{
				Debug.Log("Ray not hitting anything");
				r_ray.SetPosition(1,
				                  (r_controller_GO.transform.forward * rayRadius) + r_controller_GO.transform.position);
			}
		}

		if(OVRInput.GetUp(OVRInput.Button.Two, r_controller))
		{
			Debug.Log("B button released");
			r_ray.gameObject.SetActive(false);
			RaycastHit hit;
			Vector3 teleportLocation;
			if(Physics.Raycast(r_controller_GO.transform.position,
			                   r_controller_GO.transform.forward, out hit, rayRadius))///, rayRange))
			{
				Debug.Log("Ray hit an obstacle");
				if(hit.transform.tag == "Button")
				{
					Debug.Log("Its a Button");
					hit.collider.gameObject.GetComponent<VRButton>().click();
				}
				else
				{
					teleportLocation = hit.point;
					gameLogic.teleport(teleportLocation);
				}
			}
			else
			{
				Debug.Log("Ray didn't hit anything");
				teleportLocation = (r_controller_GO.transform.forward * rayRadius) + r_controller_GO.transform.position;
				gameLogic.teleport(teleportLocation);
			}

		}
	}
}
