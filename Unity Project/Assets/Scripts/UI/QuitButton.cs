using System;
using UnityEngine;


public class QuitButton : RiftUIElement
{
	public override void Selected()
	{
		Application.Quit();
		Debug.Log("Quitting");
	}
}