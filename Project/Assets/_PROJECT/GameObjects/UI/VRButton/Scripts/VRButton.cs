using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class VRButton : MonoBehaviour
{
	private Renderer rend;
	public Color normalColor;
	public Color highlightColor;
	public Color clickColor;
	public enum buttonType
	{
		start,
		restartLevel,
		restart,
		exit,
		switchControllers
	};
	public buttonType type;

	public GameLogic GL;

	//   S T A R T                                                                                                      
	void Start()
	{
		rend = GetComponent<Renderer>();
		Init();
	}

	public void Init()
	{
		rend.material.color = normalColor;
	}

	public void Hover()
	{
		///Debug.Log("Ray Hit on Button");
		rend.material.color = highlightColor;
	}

	public void Click()
	{
		rend.material.color = clickColor;

		if(type == buttonType.start)
			GL.StartButton();
		else if(type == buttonType.exit)
			GL.ExitButton();
		else if(type == buttonType.switchControllers)
		{
			GL.SwitchControllersButton();
			Invoke("Init", 0.125f);
		}
	}
}
