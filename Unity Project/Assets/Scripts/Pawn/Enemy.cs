using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A computer-controlled enemy with some kind of behavior each turn.
/// </summary>
public class Enemy : Pawn
{
	public List<Vector2> PathAlongFloor = new List<Vector2>();

	public float MinPathingUpdateTime = 3.0f,
				 MaxPathingUpdateTime = 6.0f;


	void Start()
	{
		UpdatePath();
		StartCoroutine(UpdatePathingCoroutine());
	}
	protected virtual void Update()
	{
		base.Update();

		if (Player.Instance == null || !Player.Instance.Controller.enabled ||
			!Controller.enabled)
			return;


		Vector3 pos = MyTransform.position;


		bool DEbug = (this == GameRegion.Instance.RoomObj.RoomPawns[0]);
		Room rm = GameRegion.Instance.RoomObj;

		//if (DEbug)
		//	Debug.Log("This: " + pos + " " + rm.PosToRoomCoord(pos) +
		//			  "; target: " + new Vector3(PathAlongFloor[0].x, 0.0f, PathAlongFloor[0].y) + " " + rm.PosToRoomCoord(new Vector3(PathAlongFloor[0].x, 0.0f, PathAlongFloor[0].y)));


		//If the path is finished, calculate a new path.
		if (PathAlongFloor.Count == 0)
		{
			UpdatePath();
		}
		//Otherwise, keep following the path.
		else
		{
			Vector2 toTarget = new Vector3(PathAlongFloor[0].x - pos.x, 0.0f,
										   PathAlongFloor[0].y - pos.z);

			//If this enemy is close enough to the target, just snap to it.
			float dist = toTarget.magnitude;
			float moveSpeed = MoveSpeed * Time.deltaTime;
			if (dist < moveSpeed)
			{
				MyTransform.position = new Vector3(PathAlongFloor[0].x, pos.y, PathAlongFloor[0].y);
				PathAlongFloor.RemoveAt(0);
			}
			//Otherwise, keep moving like normal.
			else
			{
				toTarget = toTarget.normalized * moveSpeed;
				Controller.Move(new Vector3(toTarget.x, 0.0f, toTarget.y));
			}
		}

		//Constantly attack the enemy when possible.
		Vector3 playerPos = Player.Instance.MyTransform.position;
		Vector2 toPlayer = new Vector2(playerPos.x - pos.x, playerPos.z - pos.z);
		float distToPlayer = toPlayer.magnitude;
		if (distToPlayer < GameConstants.RockAttackDist)
		{
			Attack(WeaponTypes.Rock);
		}
		else
		{
			float ratio = Mathf.Abs(toPlayer.y / toPlayer.x);
			if (ratio > 2.0f || ratio < 0.5f)
			{
				Attack(WeaponTypes.Paper);
			}
			else
			{
				Attack(WeaponTypes.Scissors);
			}
		}
	}

	private IEnumerator UpdatePathingCoroutine()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(MinPathingUpdateTime,
																 MaxPathingUpdateTime));

		UpdatePath();

		StartCoroutine(UpdatePathingCoroutine());
	}

	private void UpdatePath()
	{
		//Choose a random destination.
		List<mVector2i> freeSpaces = GameRegion.Instance.RoomObj.FreeSpaces;
		mVector2i start = GameRegion.Instance.RoomObj.PosToRoomCoord(MyTransform.position),
				  end = freeSpaces[UnityEngine.Random.Range(0, freeSpaces.Count)];
		PathAlongFloor = GameRegion.Instance.RoomObj.FindPath(start, end, false).Select(
							v2i =>
							{
								Vector3 pos = GameRegion.Instance.RoomObj.RoomCoordToPos(v2i);
								return new Vector2(pos.x, pos.z);
							}).ToList();
	}
}