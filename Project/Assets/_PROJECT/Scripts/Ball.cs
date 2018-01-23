using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private GameLogic GL;
	private Vector3 startPosition;
	///private bool isMoving;
	private Rigidbody rigidbody;
	///public bool isRolling;
	private bool ballStopped;
	private float ballStoppedDuration;
	///private AudioSource speaker;
	///public AudioClip ballAudio;

	void Start()
	{
		///speaker = GetComponent<AudioSource>();
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		rigidbody = GetComponent<Rigidbody>();
		startPosition = transform.position;
		///isRolling = false;
		ballStopped = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.transform.tag == "Ground" ||
		   collider.transform.tag == "BallReset" ||
		   collider.transform.tag == "Stage")
			GL.BallTouchedGround();
		else if(collider.transform.tag == "Finish")
			GL.BallTouchedFinish();
		else if(collider.transform.tag == "Star")
			GL.BallTouchedStar(collider.gameObject);
	}

	/*void OnCollisionEnter()
	{
		if(!rigidbody.isKinematic)
			AudioSource.PlayClipAtPoint(ballAudio, transform.position);
	}*/

	public void Reset()
	{
		transform.position = startPosition;
		transform.rotation = Quaternion.Euler(Vector3.zero);
		rigidbody.isKinematic = true;
		///isMoving = false;
		ballStopped = false;
		/*if(speaker.isPlaying)
			speaker.Stop();*/
	}

	void Update()
	{
		if(rigidbody.velocity.magnitude <= GL.ballResetSpeed && rigidbody.isKinematic == false)/// && transform.parent.gameObject == null)
		{
			if(!ballStopped)
			{
				ballStopped = true;
				ballStoppedDuration = 0;
			}
			ballStoppedDuration += Time.deltaTime;
			if(ballStoppedDuration >= GL.ballResetTime)
				GL.BallTouchedGround();
		}

		/*if(rigidbody.velocity.magnitude <= 0.1f)
			isRolling = true;
		else
			isRolling = false;*/
	}
}
