using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private GameLogic GL;
	private Vector3 startPosition;
	private bool isMoving;
	private Rigidbody rigidbody;

	void Start()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
		rigidbody = GetComponent<Rigidbody>();
		startPosition = transform.position;
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

	public void Reset()
	{
		transform.position = startPosition;
		transform.rotation = Quaternion.Euler(Vector3.zero);
		rigidbody.isKinematic = true;
		isMoving = false;
	}

	void Update()
	{
		if(rigidbody.velocity.magnitude <= GL.ballResetSpeed && rigidbody.isKinematic == false)/// && transform.parent.gameObject == null)
			GL.BallTouchedGround();
	}
}
