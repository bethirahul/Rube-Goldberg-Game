using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class FanWind : MonoBehaviour
{
	private GameObject fan_GO;
	private GameLogic GL;
	public GameObject blades_GO;
	public GameObject helper_GO;
	private float airDistance;

	void Start()
	{
		fan_GO = transform.parent.gameObject;
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		airDistance = GetComponent<CapsuleCollider>().height;
	}

	void Update()
	{
		//  Rotate Blades
		if(GL.isGameStarted)
		{
			blades_GO.transform.Rotate(new Vector3(0, 0, 10f));
			if(helper_GO.activeSelf)
				helper_GO.SetActive(false);
		}
		else
		{
			blades_GO.transform.Rotate(new Vector3(0, 0, 0.25f));
			if(!helper_GO.activeSelf)
				helper_GO.SetActive(true);
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if(collider.transform.tag == "Throwable")/// && GL.isGameStarted)
		{
			Debug.Log(collider.gameObject.name + " is in fan area");
			float dist = Vector3.Distance(fan_GO.transform.position, collider.transform.position);
			if(dist < airDistance)
			{
				dist = airDistance - dist;
				collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.up)
															* GL.fanSpeed * (dist / airDistance));
			}
			Debug.Log("Velocity changed to " + collider.GetComponent<Rigidbody>().velocity);
		}
	}
}
