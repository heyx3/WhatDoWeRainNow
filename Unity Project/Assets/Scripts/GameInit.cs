using System;
using UnityEngine;


/// <summary>
/// Handles initialization of game.
/// </summary>
public class GameInit : MonoBehaviour
{
	public float WaitTime = 5.0f;

	void Start()
	{
		StartCoroutine(StartGameCoroutine());
	}

	private System.Collections.IEnumerator StartGameCoroutine()
	{
		yield return new WaitForSeconds(WaitTime);

		GameRegion.Instance.SwitchRooms(new mVector2i(0, 0));
	}
}