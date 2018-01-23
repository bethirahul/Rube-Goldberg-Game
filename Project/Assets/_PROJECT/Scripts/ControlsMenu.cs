using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is to show controller's infor with a button to switch controllers
// description changes when controllers change

public class ControlsMenu : MonoBehaviour
{
	public GameObject info_A;
	public GameObject info_B;
	private ControllerLayout controllerLayout;

	void Start()
	{
		controllerLayout = GameObject.Find("ControllerLayout").GetComponent<ControllerLayout>();
		///info_A = GameObject.Find("ControlsMenu_UI/ControlsMenu_Canvas/Info_Text_A");
		///info_B = GameObject.Find("ControlsMenu_UI/ControlsMenu_Canvas/Info_Text_B");

		ChangeControllersInfo();
	}

	public void ChangeControllersInfo()
	{
		if(controllerLayout.layout == ControllerLayout.layoutEnum.normal)
		{
			info_A.SetActive(true);
			info_B.SetActive(false);
		}
		else
		{
			info_A.SetActive(false);
			info_B.SetActive(true);
		}
	}
}
