using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLayout : MonoBehaviour
{
	public enum layoutEnum
	{
		normal,
		reversed
	};
	public layoutEnum layout;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		layout = layoutEnum.normal;
	}
}
