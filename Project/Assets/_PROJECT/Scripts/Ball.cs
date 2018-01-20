using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private GameLogic GL;
	private Vector3 startPosition;
	private Vector3[] lastPositions;
	private bool isMoving;

	void Start()
	{
		GL = GameObject.Find("GameLogic").GetComponent<GameLogic>();
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
		GetComponent<Rigidbody>().isKinematic = true;
		isMoving = false;
	}

	void Update()
	{
		if(isMoving)
		{
			
		}
	}
}
