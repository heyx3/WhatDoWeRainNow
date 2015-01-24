using System;
using UnityEngine;


public class GameRegion : MonoBehaviour
{
	public static GameRegion Instance { get; private set; }


	public float RegionScale = 1.0f;
	public int nRoomsWidth = 5,
			   nRoomsHeight = 5;

	public float ChangeRoomsTime = 4.0f;


	public bool[,] RoomsCleared { get; private set; }
	public mVector2i CurrentRoom { get; private set; }


	private Transform tr;
	private Room roomGen;


	/// <summary>
	/// Gets the center of the given room.
	/// </summary>
	public Vector3 RoomCoordToPos(mVector2i roomCoord)
	{
		Vector3 pos = tr.position;
		Vector3 roomSize = new Vector3(RegionScale * nRoomsWidth, 0.0f, RegionScale * nRoomsHeight);
		return new Vector3(Mathf.Lerp(pos.x - (roomSize.x * 0.5f), pos.x + (roomSize.x * 0.5f),
									  (roomCoord.X / (float)nRoomsWidth)),
						   pos.y,
						   Mathf.Lerp(pos.z - (roomSize.z * 0.5f), pos.z + (roomSize.z * 0.5f),
									  (roomCoord.Y / (float)nRoomsHeight)));
	}
	/// <summary>
	/// Switches play to the given next room.
	/// Logs an error if the given room has already been visited.
	/// </summary>
	public void SwitchRooms(mVector2i newRoom)
	{
		mVector2i oldRoom = CurrentRoom;

		Vector3 newPos = RoomCoordToPos(newRoom);
		CurrentRoom = newRoom;

		//Set up the camera motion.
		TweenPosition tweener = Player.Instance.gameObject.AddComponent<TweenPosition>();
		tweener.Target = Player.Instance.GetTargetPosition(newRoom);
		tweener.MoveTime = ChangeRoomsTime;

		//Set up the room.
		roomGen.GenerateNewRoom(oldRoom, newRoom);
	}


	void Awake()
	{
		Instance = this;
		tr = transform;
		roomGen = FindObjectOfType<Room>();
	}

	void OnDrawGizmos()
	{
		tr = transform;

		Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		Vector3 size = new Vector3(nRoomsWidth * RegionScale,
								   0.5f,
								   nRoomsHeight * RegionScale);
		Gizmos.DrawCube(tr.position, size);
	}
}