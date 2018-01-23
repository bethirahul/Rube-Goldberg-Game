using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

// This is for fan wind behaviour, this also disables the helper representing air to not show when ball is thrown

public class FanWind : MonoBehaviour
{
	private GameObject fan_GO;
	private GameLogic GL;
	public GameObject blades_GO;
	public GameObject helper_GO;
	private float airDistance;
	public AudioSource speaker;

	void Start()
	{
		fan_GO = transform.parent.gameObject;
		///speaker = GetComponent<AudioSource>();
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		airDistance = GetComponent<CapsuleCollider>().height;
	}

	void Update()
	{
		//  Rotate Blades
		if(GL.isGameStarted)
		{
			blades_GO.transform.Rotate(new Vector3(0, 0, 15f)); // rotate blades at highspeed and make sound when ball is thrown from stage
			if(helper_GO.activeSelf)
				helper_GO.SetActive(false);
			if(!speaker.isPlaying)
				speaker.Play();
		}
		else
		{
			blades_GO.transform.Rotate(new Vector3(0, 0, 0.25f)); // move fan blades slowly and stop sound when ball gets reset
			if(!helper_GO.activeSelf)
				helper_GO.SetActive(true);
			if(speaker.isPlaying)
				speaker.Stop();
		}
	}

	void OnTriggerStay(Collider collider)
	{
		// if a throwable objects comes into fan air path, calculate the intensity with respect to distance to the fan
		if(collider.transform.tag == "Throwable")/// && GL.isGameStarted) 
		{
			///Debug.Log(collider.gameObject.name + " is in fan area");
			float dist = Vector3.Distance(fan_GO.transform.position, collider.transform.position);
			if(dist < airDistance)
			{
				dist = airDistance - dist;
				collider.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.up)
															* GL.fanSpeed * (dist / airDistance));
			}
			///Debug.Log("Velocity changed to " + collider.GetComponent<Rigidbody>().velocity);
		}
	}
}
