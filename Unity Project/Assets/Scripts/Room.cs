using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Manages the creation and destruction of rooms.
/// </summary>
public class Room : MonoBehaviour
{
	public bool[,] RoomObstacles;

	/// <summary>
	/// Does NOT include the player.
	/// </summary>
	[NonSerialized]
	public List<Pawn> RoomPawns = new List<Pawn>();
	/// <summary>
	/// All obstacles currently in the field.
	/// </summary>
	[NonSerialized]
	public List<Collider> Obstacles = new List<Collider>();
	/// <summary>
	/// The free spaces in this room.
	/// </summary>
	[NonSerialized]
	public List<mVector2i> FreeSpaces = new List<mVector2i>();


	public List<GameObject> ObstaclePrefabs = new List<GameObject>(),
							EnemyPrefabs = new List<GameObject>();
	public GameObject PlayerPrefab, FloorPrefab;

	public float PercentSpacesFilled = 0.2f;
	public int MinRoomSize = 5,
			   MaxRoomSize = 8;

	public int NEnemiesMin = 1,
			   NEnemiesMax = 4;

	public bool DEBUG_KillAllEnemies = false;


	private GameRegion Game { get { return GameRegion.Instance; } }


	private List<GameObject> invisibleWalls = new List<GameObject>();
	private bool RoomNeedsCleaning = false;


	void Update()
	{
		if (DEBUG_KillAllEnemies)
		{
			DEBUG_KillAllEnemies = false;
			foreach (Pawn p in RoomPawns)
				Destroy(p.gameObject);
			RoomPawns.Clear();
		}
	}


	/// <summary>
	/// Gets the center of the tile at the given coordiante in this room.
	/// </summary>
	public Vector3 RoomCoordToPos(mVector2i roomCoord)
	{
		Vector3 pos = Game.RoomCoordToPos(Game.CurrentRoom);

		int roomSize = RoomObstacles.GetLength(0);

		Vector2 lerpCoord = new Vector2(roomCoord.X / (float)roomSize,
									    roomCoord.Y / (float)roomSize);
		//lerpCoord = new Vector2((lerpCoord.x * 2.0f) - 1.0f, (lerpCoord.y * 2.0f) - 1.0f);

		float halfTileSize = 0.5f * (Game.RegionScale / RoomObstacles.GetLength(0));
		Vector2 min = new Vector2(pos.x - (roomSize * halfTileSize),
								  pos.z - (roomSize * halfTileSize)),
				max = new Vector2(pos.x + (roomSize * halfTileSize),
								  pos.z + (roomSize * halfTileSize));

		return new Vector3(Mathf.Lerp(min.x, max.x, lerpCoord.x) + halfTileSize,
						   0.0f,
						   Mathf.Lerp(min.y, max.y, lerpCoord.y) + halfTileSize);
	}
	/// <summary>
	/// Gets the room coordinate of the tile that the given position is inside.
	/// </summary>
	public mVector2i PosToRoomCoord(Vector3 pos)
	{
		Vector3 thisPos = Game.RoomCoordToPos(Game.CurrentRoom);

		int roomSize = RoomObstacles.GetLength(0);
		
		float halfRoomSize = Game.RegionScale * 0.5f;
		Vector2 min = new Vector2(thisPos.x - halfRoomSize,
								  thisPos.z - halfRoomSize),
				max = new Vector2(thisPos.x + halfRoomSize,
								  thisPos.z + halfRoomSize);
		float halfTileSize = halfRoomSize / RoomObstacles.GetLength(0);


		Vector2 lerpCoord = new Vector2(Mathf.InverseLerp(min.x, max.x, pos.x),
										Mathf.InverseLerp(min.y, max.y, pos.z));

		return new mVector2i((int)(lerpCoord.x * roomSize),
							 (int)(lerpCoord.y * roomSize));
	}

	/// <summary>
	/// Finds a path from the given start to the given end.
	/// Returns "null" if there is none thanks to blockers.
	/// </summary>
	public List<mVector2i> FindPath(mVector2i start, mVector2i end, bool canMoveDiagonal)
	{
		GameGraph graph = new GameGraph(canMoveDiagonal);
		PathFinder<GameNode> pather = new PathFinder<GameNode>(graph, (n1, n2) => new GameEdge(n1, n2));
		
		pather.Start = new GameNode(start);
		pather.End = new GameNode(end);

		pather.FindPath();
		if (pather.CurrentPath[pather.CurrentPath.Count - 1].GridCoord != end)
			return null;
		
		return pather.CurrentPath.Select(n => n.GridCoord).ToList();
	}

	/// <summary>
	/// Sets up a new room at the given new grid position.
	/// Automatically cleans up the given current room if it hasn't been cleaned yet.
	/// Returns all the empty positions in the room.
	/// </summary>
	public void GenerateNewRoom(mVector2i oldRoom, mVector2i newRoom)
	{
		if (RoomNeedsCleaning)
		{
			CleanupCurrentRoom(oldRoom);
		}

		//Keep re-generating the room until a valid one is created.
		//Count the number of times this loop happens to prevent infinite loops.
		List<mVector2i> freeSpaces = null;
		int nIterations = 0;
		const int nTries = 40;
		int roomSize = -1;
		while (true)
		{
			freeSpaces = FillRoom(out roomSize);

			if (IsRoomOpen(roomSize, freeSpaces))
				break;

			nIterations += 1;
			if (nIterations >= nTries)
			{
				Debug.LogError("Can't generate a valid room!");
				Application.Quit();
				break;
			}
		}

		FreeSpaces = freeSpaces;

		
		Vector3 thisPos = Game.RoomCoordToPos(Game.CurrentRoom);
		float objScale = Game.RegionScale / (float)roomSize;


		//Generate obstacles.
		for (int x = 0; x < roomSize; ++x)
		{
			for (int y = 0; y < roomSize; ++y)
			{
				if (RoomObstacles[x, y])
				{
					GameObject toUse = ObstaclePrefabs[UnityEngine.Random.Range(0, ObstaclePrefabs.Count)];
					Transform objTr = ((GameObject)Instantiate(toUse)).transform;
					objTr.localScale *= objScale;
					
					Vector3 floorPos = RoomCoordToPos(new mVector2i(x, y));
					objTr.position = new Vector3(floorPos.x, objTr.position.y, floorPos.z);

					Obstacles.Add(objTr.collider);
				}
			}
		}


		//Put the player in a random free space.
		int index = UnityEngine.Random.Range(0, freeSpaces.Count);
		Transform playerTr = ((GameObject)Instantiate(PlayerPrefab)).transform;
		playerTr.position = RoomCoordToPos(freeSpaces[index]);
		playerTr.localScale *= objScale;


		//Generate enemies in other random free spaces.
		RoomPawns = new List<Pawn>();
		int minArea = MinRoomSize * MinRoomSize,
			maxArea = MaxRoomSize * MaxRoomSize;
		int nEnemies = Mathf.RoundToInt(Mathf.Lerp((float)NEnemiesMin, (float)NEnemiesMax,
												   Mathf.InverseLerp((float)minArea, (float)maxArea,
													 				 (float)(roomSize * roomSize))));
		nEnemies = Mathf.Min(nEnemies, freeSpaces.Count - 1);
		if (oldRoom.X == -1 && oldRoom.Y == -1)
			nEnemies = 1;
		List<int> usedIndices = new List<int>() { index };
		for (int i = 0; i < nEnemies; ++i)
		{
			index = usedIndices[0];
			while (usedIndices.Contains(index))
				index = UnityEngine.Random.Range(0, freeSpaces.Count);

			usedIndices.Add(index);
			GameObject prefab = EnemyPrefabs[UnityEngine.Random.Range(0, EnemyPrefabs.Count)];
			Transform pawnTr = ((GameObject)Instantiate(prefab)).transform;
			pawnTr.position = RoomCoordToPos(freeSpaces[index]);
			pawnTr.localScale *= objScale;
			RoomPawns.Add(pawnTr.GetComponent<Pawn>());
		}
		

		//Place down the floor.
		Transform floorTr = ((GameObject)Instantiate(FloorPrefab)).transform;
		floorTr.position = thisPos + new Vector3(0.0f, 0.005f, 0.0f);
		floorTr.localScale *= Game.RegionScale;


		//Create invisible walls.

		float halfRoomSize = Game.RegionScale * 0.5f;
		Vector2 min = new Vector2(thisPos.x - halfRoomSize,
								  thisPos.z - halfRoomSize),
				max = new Vector2(thisPos.x + halfRoomSize,
								  thisPos.z + halfRoomSize);

		float wallSize = 1.0f * Game.RegionScale;
		invisibleWalls.Add(new GameObject("Left Wall"));
		invisibleWalls[0].transform.position = thisPos + new Vector3(-halfRoomSize - (wallSize * 0.5f), 0.0f, 0.0f);
		invisibleWalls.Add(new GameObject("Right Wall"));
		invisibleWalls[1].transform.position = thisPos + new Vector3(halfRoomSize + (wallSize * 0.5f), 0.0f, 0.0f);
		invisibleWalls.Add(new GameObject("Top Wall"));
		invisibleWalls[2].transform.position = thisPos + new Vector3(0.0f, 0.0f, -halfRoomSize - (wallSize * 0.5f));
		invisibleWalls.Add(new GameObject("Bottom Wall"));
		invisibleWalls[3].transform.position = thisPos + new Vector3(0.0f, 0.0f, halfRoomSize + (wallSize * 0.5f));
		foreach (GameObject wall in invisibleWalls)
			wall.AddComponent<BoxCollider>().size = new Vector3(wallSize, 1.0f, wallSize);


		RoomNeedsCleaning = true;
	}
	private List<mVector2i> FillRoom(out int outRoomSize)
	{
		List<mVector2i> freeSpaces = new List<mVector2i>();

		//Generate the spaces.
		outRoomSize = UnityEngine.Random.Range(MinRoomSize, MaxRoomSize);
		RoomObstacles = new bool[outRoomSize, outRoomSize];
		float objScale = Game.RegionScale / (float)outRoomSize;
		for (int x = 0; x < outRoomSize; ++x)
		{
			for (int y = 0; y < outRoomSize; ++y)
			{
				if (UnityEngine.Random.value < PercentSpacesFilled)
				{
					RoomObstacles[x, y] = true;
				}
				else
				{
					freeSpaces.Add(new mVector2i(x, y));
					RoomObstacles[x, y] = false;
				}
			}
		}

		return freeSpaces;
	}
	private bool IsRoomOpen(int roomSize, List<mVector2i> freeSpaces)
	{
		if (freeSpaces.Count < 2)
			return false;

		//Pick a random empty spot and flood-fill the area to see what's connected.
		//If there are any spots that weren't touched by the flood-fill AND
		//    are a free space, then the room isn't fully open.

		bool[,] isReached = new bool[roomSize, roomSize];
		for (int x = 0; x < isReached.GetLength(0); ++x)
			for (int y = 0; y < isReached.GetLength(1); ++y)
				isReached[x, y] = false;

		Stack<mVector2i> searchFrontier = new Stack<mVector2i>();
		searchFrontier.Push(freeSpaces[0]);
		isReached[freeSpaces[0].X, freeSpaces[0].Y] = true;
		while (searchFrontier.Count > 0)
		{
			mVector2i searchPos = searchFrontier.Pop();

			if (searchPos.X > 0)
			{
				if (!isReached[searchPos.X - 1, searchPos.Y] &&
					freeSpaces.Contains(searchPos.LessX()))
				{
					isReached[searchPos.X - 1, searchPos.Y] = true;
					searchFrontier.Push(searchPos.LessX());
				}
			}
			if (searchPos.Y > 0)
			{
				if (!isReached[searchPos.X, searchPos.Y - 1] &&
					freeSpaces.Contains(searchPos.LessY()))
				{
					isReached[searchPos.X, searchPos.Y - 1] = true;
					searchFrontier.Push(searchPos.LessY());
				}
			}
			if (searchPos.X < isReached.GetLength(0) - 1)
			{
				if (!isReached[searchPos.X + 1, searchPos.Y] &&
					freeSpaces.Contains(searchPos.MoreX()))
				{
					isReached[searchPos.X + 1, searchPos.Y] = true;
					searchFrontier.Push(searchPos.MoreX());
				}
			}
			if (searchPos.Y < isReached.GetLength(1) - 1)
			{
				if (!isReached[searchPos.X, searchPos.Y + 1] &&
					freeSpaces.Contains(searchPos.MoreY()))
				{
					isReached[searchPos.X, searchPos.Y + 1] = true;
					searchFrontier.Push(searchPos.MoreY());
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

		//Remove the room's obstacles from the physics engine.
		Obstacles.ForEach(c => Destroy(c));
		Obstacles.Clear();

		invisibleWalls.ForEach(w => Destroy(w));
		invisibleWalls.Clear();

		RoomObstacles = null;

		Destroy(Player.Instance.gameObject);
	}
}