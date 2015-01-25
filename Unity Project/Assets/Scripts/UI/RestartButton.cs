using UnityEngine;


public class RestartButton : RiftUIElement
{
	public override void Selected()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
}