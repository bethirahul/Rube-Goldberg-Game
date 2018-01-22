using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class FanWind : MonoBehaviour
{
	private GameObject fan_GO;
	private GameLogic GL;
	public GameObject blades_GO;

	void Start()
	{
		fan_GO = transform.parent.gameObject;
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
	}

	void Update()
	{
		//  Rotate Blades
		if(GL.isGameStarted)
		{
			blades_GO.transform.Rotate(new Vector3(0, 0, 10f));
		}
		else
		{
			blades_GO.transform.Rotate(new Vector3(0, 0, 0.5f));
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if(collider.transform.tag == "Throwable")/// && GL.isGameStarted)
		{
			Debug.Log(collider.gameObject.name + " is in fan area");
			collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.up) * GL.fanSpeed);
			Debug.Log("Velocity changed to " + collider.GetComponent<Rigidbody>().velocity);
		}
	}
}
