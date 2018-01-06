using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
	//  OCULUS
	public OVRInput.Controller l_controller;
	public OVRInput.Controller r_controller;
	public GameObject l_controller_GO;
	public GameObject r_controller_GO;

	//  RAY CASTING
	public LineRenderer l_ray;
	public LineRenderer r_ray;
	///public LayerMask rayRange;
	public float rayRadius;

	//   S T A R T                                                                                                      
	void Start()
	{
		l_ray.gameObject.SetActive(false);
		r_ray.gameObject.SetActive(false);
	}
	
	//   U P D A T E                                                                                                    
	void Update()
	{
		if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, l_controller))
		{
			///Debug.Log("Left Index Trigger is being pressed");
			l_ray.gameObject.SetActive(true);
		}

		if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, r_controller))
		{
			///Debug.Log("Right Index Trigger is being pressed");
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
					hit.collider.gameObject.GetComponent<VRButton>().buttonClick();
				}
			}
			else
			{
				Debug.Log("Ray not hitting anything");
				r_ray.SetPosition(1,
								  (r_controller_GO.transform.forward * rayRadius) + r_controller_GO.transform.position);
			}
		}

		if(OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, l_controller))
		{
			///Debug.Log("Left Index Trigger released");
			l_ray.gameObject.SetActive(false);
		}

		if(OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, r_controller))
		{
			///Debug.Log("Right Index Trigger released");
			r_ray.gameObject.SetActive(false);
		}
	}
}
