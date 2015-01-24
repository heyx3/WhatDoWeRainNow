using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages the creation and destruction of rooms.
/// </summary>
public class Room : MonoBehaviour
{
	public GameObject[,] RoomGrid { get; private set; }


	public List<GameObject> ObstaclePrefabs = new List<GameObject>();

	public float PercentSpacesFilled = 0.2f;
	public int MinRoomSize = 5,
			   MaxRoomSize = 8;


	private GameRegion Game { get { return GameRegion.Instance; } }

	private bool RoomNeedsCleaning = false;


	/// <summary>
	/// Sets up a new room at the given new grid position.
	/// Automatically cleans up the given current room if it hasn't been cleaned yet.
	/// Returns all the empty positions in the room.
	/// </summary>
	public List<mVector2i> GenerateNewRoom(mVector2i oldRoom, mVector2i newRoom)
	{
		if (RoomNeedsCleaning)
		{
			CleanupCurrentRoom(oldRoom);
		}

		//Keep re-generating the room until a valid one is created.
		//Count the number of times this loop happens to prevent infinite loops.
		List<mVector2i> freeSpaces = null;
		int i = 0;
		const int nTries = 40;
		while (true)
		{
			freeSpaces = FillRoom();
			if (IsRoomOpen(freeSpaces))
				break;

			i += 1;
			if (i >= nTries)
			{
				Application.Quit();
				Debug.LogError("Can't generate a valid room!");
				return null;
			}
		}

		return freeSpaces;
	}
	private List<mVector2i> FillRoom()
	{
		List<mVector2i> freeSpaces = new List<mVector2i>();

		int roomSize = UnityEngine.Random.Range(MinRoomSize, MaxRoomSize);
		RoomGrid = new GameObject[roomSize, roomSize];
		float objScale = 1.0f / (float)roomSize;
		for (int x = 0; x < roomSize; ++x)
		{
			for (int y = 0; y < roomSize; ++y)
			{
				if (UnityEngine.Random.value < PercentSpacesFilled)
				{
					GameObject toUse = ObstaclePrefabs[UnityEngine.Random.Range(0, ObstaclePrefabs.Count)];
					Transform objTr = ((GameObject)Instantiate(toUse)).transform;
					objTr.localScale *= objScale;
				}
				else
				{
					RoomGrid[x, y] = null;
					freeSpaces.Add(new mVector2i(x, y));
				}
			}
		}

		return freeSpaces;
	}
	private bool IsRoomOpen(List<mVector2i> freeSpaces)
	{
		if (freeSpaces.Count == 0) return false;

		//Pick a random empty spot and flood-fill the area to see what's connected.
		//If there are any spots that weren't touched by the flood-fill AND
		//    are a free space, then the room isn't fully open.

		bool[,] isReached = new bool[RoomGrid.GetLength(0), RoomGrid.GetLength(1)];
		for (int x = 0; x < isReached.GetLength(0); ++x)
			for (int y = 0; y < isReached.GetLength(1); ++y)
				isReached[x, y] = false;

		List<mVector2i> searchFrontier = new List<mVector2i>() { freeSpaces[0] };
		isReached[freeSpaces[0].X, freeSpaces[0].Y] = true;
		while (searchFrontier.Count > 0)
		{
			mVector2i searchPos = searchFrontier[searchFrontier.Count - 1];
			searchFrontier.RemoveAt(searchFrontier.Count - 1);

			if (searchPos.X > 0)
			{
				if (!isReached[searchPos.X - 1, searchPos.Y] &&
					freeSpaces.Contains(searchPos.LessX()))
				{
					isReached[searchPos.X - 1, searchPos.Y] = true;
					searchFrontier.Add(searchPos.LessX());
				}
			}
			if (searchPos.Y > 0)
			{
				if (!isReached[searchPos.X, searchPos.Y - 1] &&
					freeSpaces.Contains(searchPos.LessY()))
				{
					isReached[searchPos.X, searchPos.Y - 1] = true;
					searchFrontier.Add(searchPos.LessY());
				}
			}
			if (searchPos.X < isReached.GetLength(0) - 1)
			{
				if (!isReached[searchPos.X + 1, searchPos.Y] &&
					freeSpaces.Contains(searchPos.MoreX()))
				{
					isReached[searchPos.X + 1, searchPos.Y] = true;
					searchFrontier.Add(searchPos.MoreX());
				}
			}
			if (searchPos.Y < isReached.GetLength(1) - 1)
			{
				if (!isReached[searchPos.X, searchPos.Y + 1] &&
					freeSpaces.Contains(searchPos.MoreX()))
				{
					isReached[searchPos.X, searchPos.Y + 1] = true;
					searchFrontier.Add(searchPos.MoreY());
				}
			}
		}


		for (int x = 0; x < isReached.GetLength(0); ++x)
			for (int y = 0; y < isReached.GetLength(1); ++y)
				if (!isReached[x, y] && freeSpaces.Contains(new mVector2i(x, y)))
					return false;
		return true;
	}

	/// <summary>
	/// Finishes up the current room.
	/// </summary>
	public void CleanupCurrentRoom(mVector2i room)
	{
		RoomNeedsCleaning = false;
	}
}