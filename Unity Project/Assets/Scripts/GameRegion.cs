using System;
using UnityEngine;


public class GameRegion : MonoBehaviour
{
	public static GameRegion Instance { get; private set; }


	/// <summary>
	/// The size of a room, in Unity units.
	/// </summary>
	public float RegionScale = 1.0f;
	public int nRoomsWidth = 5,
			   nRoomsHeight = 5;

	public float ChangeRoomsTime = 4.0f;

	
	public Room RoomObj { get; private set; }
	public bool[,] RoomsCleared { get; private set; }
	public mVector2i CurrentRoom { get; private set; }


	private Transform tr;


	/// <summary>
	/// Gets the center of the given room.
	/// </summary>
	public Vector3 RoomCoordToPos(mVector2i roomCoord)
	{
		Vector3 pos = tr.position;
		
		Vector2 lerpCoord = new Vector2(roomCoord.X / (float)nRoomsWidth,
									    roomCoord.Y / (float)nRoomsHeight);
		//lerpCoord = new Vector2((lerpCoord.x * 2.0f) - 1.0f, (lerpCoord.y * 2.0f) - 1.0f);

		float halfRoomSize = 0.5f * RegionScale;
		Vector2 min = new Vector2(pos.x - (nRoomsWidth * halfRoomSize),
								  pos.z - (nRoomsHeight * halfRoomSize)),
				max = new Vector2(pos.x + (nRoomsWidth * halfRoomSize),
								  pos.z + (nRoomsHeight * halfRoomSize));
		min += new Vector2(halfRoomSize, halfRoomSize);
		max -= new Vector2(halfRoomSize, halfRoomSize);

		return new Vector3(Mathf.Lerp(min.x, max.x, lerpCoord.x),
						   0.0f,
						   Mathf.Lerp(min.y, max.y, lerpCoord.y));



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
		TweenPosition tweener = PlayerCamera.Instance.gameObject.AddComponent<TweenPosition>();
		tweener.Target = PlayerCamera.Instance.GetTargetPosition(newRoom);
		tweener.MoveTime = ChangeRoomsTime;

		//Set up the room.
		RoomObj.GenerateNewRoom(oldRoom, newRoom);
	}


	void Awake()
	{
		Instance = this;
		tr = transform;
		RoomObj = FindObjectOfType<Room>();
		RoomsCleared = new bool[nRoomsWidth, nRoomsHeight];
		for (int x = 0; x < nRoomsWidth; ++x)
			for (int y = 0; y < nRoomsHeight; ++y)
				RoomsCleared[x, y] = false;
	}
	void Update()
	{
		if (RoomObj.RoomPawns.Count == 0 && Player.Instance != null)
		{
			RoomsCleared[CurrentRoom.X, CurrentRoom.Y] = true;
			mVector2i newRoom = CurrentRoom;
			while (RoomsCleared[newRoom.X, newRoom.Y])
				newRoom = new mVector2i(UnityEngine.Random.Range(0, RoomsCleared.GetLength(0)),
										UnityEngine.Random.Range(0, RoomsCleared.GetLength(1)));
			SwitchRooms(newRoom);
		}
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