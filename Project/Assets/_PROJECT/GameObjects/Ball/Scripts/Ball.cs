using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public GameLogic GL;

	/*void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}*/

	void OnTriggerEnter(Collider collider)
	{
		if(collider.transform.tag == "Ground" || collider.transform.tag == "BallReset")
			GL.BallTouchedGround();
		else if(collider.transform.tag == "Finish")
			GL.BallTouchedFinish();
	}
}
