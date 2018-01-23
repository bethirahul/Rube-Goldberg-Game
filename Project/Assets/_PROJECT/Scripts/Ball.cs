using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is for ball to detect collisions with ball, such as goal, fan, ground, etc

public class Ball : MonoBehaviour
{
	private GameLogic GL;
	private Vector3 startPosition;
	///private bool isMoving;
	private Rigidbody rigidbody;
	///public bool isRolling;
	private bool ballStopped; // this is to determine if the ball got stuck
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
		{
			GL.BallTouchedGround();
			///GL.DisplayMessage("Ball fell to the ground");
		}
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
		// If ball speed is less, then start timer, if timer runs out, reset ball
		if(rigidbody.velocity.magnitude <= GL.ballResetSpeed && rigidbody.isKinematic == false)/// && transform.parent.gameObject == null)
		{
			if(!ballStopped)
			{
				ballStopped = true;
				ballStoppedDuration = 0;
			}
			ballStoppedDuration += Time.deltaTime;
			if(ballStoppedDuration >= GL.ballResetTime)
			{
				GL.BallTouchedGround();
				GL.DisplayMessage("Ball got stuck!");
			}
		}

		/*if(rigidbody.velocity.magnitude <= 0.1f)
			isRolling = true;
		else
			isRolling = false;*/
	}
}
