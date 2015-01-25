using System;
using UnityEngine;
using GamepadInput;


public class PlayerCamera : MonoBehaviour
{
	public static PlayerCamera Instance { get; private set; }


	public Transform MyTransform { get; private set; }
	public Transform FaceTracker { get; private set; }


	public float UIHighlightScale = 1.5f;
	public Color UIColorScale = new Color(0.8f, 0.8f, 0.8f, 0.8f);


	public Vector3 RoomOffset = new Vector3(-2.0f, 2.0f, 0.0f);
	public RestartButton RestartButton;


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
					ScaleColor(pointedAtBefore.renderer.material, false);
				}
			}
			else
			{
				if (pointedAtBefore == null)
				{
					pointingAt.MyTransform.localScale *= UIHighlightScale;
					ScaleColor(pointingAt.renderer.material, true);
				}
				else if (pointedAtBefore != pointingAt)
				{
					pointingAt.MyTransform.localScale *= UIHighlightScale;
					ScaleColor(pointingAt.renderer.material, true);
					pointedAtBefore.MyTransform.localScale /= UIHighlightScale;
					ScaleColor(pointedAtBefore.renderer.material, false);
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

	private void ScaleColor(Material mat, bool scaleForward)
	{
		Color col = mat.color;
		if (scaleForward)
			mat.color = new Color(col.r * UIColorScale.r, col.g * UIColorScale.g, col.b * UIColorScale.b, col.a * UIColorScale.a);
		else mat.color = new Color(col.r / UIColorScale.r, col.g / UIColorScale.g, col.b / UIColorScale.b, col.a / UIColorScale.a);
	}
}