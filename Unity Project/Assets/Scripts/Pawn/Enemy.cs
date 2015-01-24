using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A computer-controlled enemy with some kind of behavior each turn.
/// </summary>
public abstract class Enemy : Pawn
{
	public List<Vector2> PathAlongFloor = new List<Vector2>();

	public float MinPathingUpdateTime = 3.0f,
				 MaxPathingUpdateTime = 6.0f;


	void Start()
	{

	}
	protected virtual void Update()
	{
		base.Update();

		//Follow the path.
		Vector3 pos = MyTransform.position;
		Vector2 toTarget = new Vector3(PathAlongFloor[0].x - pos.x, 0.0f,
									   PathAlongFloor[0].y - pos.z);
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

	}
}