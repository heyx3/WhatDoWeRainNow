using System;
using UnityEngine;
using GamepadInput;


public class PlayerCamera : MonoBehaviour
{
	public static PlayerCamera Instance { get; private set; }


	public Transform MyTransform { get; private set; }
	public Transform FaceTracker { get; private set; }


	public float UIHighlightScale = 1.5f;
	public Vector3 RoomOffset = new Vector3(-1.0f, 1.0f, 0.0f);


	private RiftUIElement pointedAtBefore = null;


	void Awake()
	{
		Instance = this;
		MyTransform = transform;
		FaceTracker = MyTransform.FindChild("CenterEyeAnchor");
	}
	void Update()
	{
		GamepadState gState = GamePad.GetState(GamePad.Index.Any);
		if (gState.A || Input.GetKey(KeyCode.Space))
		{
			SelectUI();
		}
		else
		{
			RiftUIElement pointingAt = GetPointedUI();
			if (pointingAt == null)
			{
				if (pointedAtBefore != null)
				{
					pointedAtBefore.MyTransform.localScale /= UIHighlightScale;
				}
			}
			else
			{
				if (pointedAtBefore == null)
				{
					pointingAt.MyTransform.localScale *= UIHighlightScale;
				}
				else if (pointedAtBefore != pointingAt)
				{
					pointingAt.MyTransform.localScale *= UIHighlightScale;
					pointedAtBefore.MyTransform.localScale /= UIHighlightScale;
				}
			}

			pointedAtBefore = pointingAt;
		}
	}

	
	/// <summary>
	/// Gets the target position for this player camera
	/// given the target room to focus on.
	/// </summary>
	public Vector3 GetTargetPosition(mVector2i roomCoord)
	{
		Vector3 floorPos = GameRegion.Instance.RoomCoordToPos(roomCoord);

		return floorPos + RoomOffset;
	}


	private RiftUIElement GetPointedUI()
	{
		Ray uiRay = new Ray(FaceTracker.position, FaceTracker.forward);
		RaycastHit outHit;
		int castLayer = (1 << LayerMask.NameToLayer("Rift UI"));

		if (Physics.Raycast(uiRay, out outHit, 9999.0f, castLayer))
			return outHit.collider.GetComponent<RiftUIElement>();
		return null;
	}

	private void SelectUI()
	{
		RiftUIElement uiObj = GetPointedUI();
		pointedAtBefore = uiObj;

		if (uiObj == null)
			return;

		uiObj.Selected();
	}
}