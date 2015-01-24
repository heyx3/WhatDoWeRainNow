using System;
using UnityEngine;


public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }


	public Transform MyTransform { get; private set; }
	public Transform FaceTracker { get; private set; }


	/// <summary>
	/// Gets the target position for this player camera
	/// given the target room to focus on.
	/// </summary>
	public Vector3 GetTargetPosition(mVector2i roomCoord)
	{
		Vector3 floorPos = GameRegion.Instance.RoomCoordToPos(roomCoord);

		Vector3 pos = MyTransform.position,
				lookDir = MyTransform.forward;

		float height = pos.y;

		float lookDirY = lookDir.y;
		Vector3 lookDirXZ = new Vector3(lookDir.x, 0.0f, lookDir.z);

		float xzToY = lookDirXZ.magnitude / lookDirY;

		Vector3 offset = -lookDirXZ * (height * xzToY);
		offset.y = height;

		return floorPos + offset;
	}


	void Awake()
	{
		Instance = this;
		MyTransform = transform;

		FaceTracker = MyTransform.FindChild("CenterEyeAnchor");
	}
}