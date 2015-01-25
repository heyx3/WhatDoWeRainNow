using System;
using UnityEngine;


public class Attack : MonoBehaviour
{
	/// <summary>
	/// Gets the horizontal component of the given vector.
	/// </summary>
	protected static Vector2 Horizontal(Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	
	[NonSerialized]
	public bool CameFromPlayer;


	protected System.Collections.IEnumerator KillAfterTimeCoroutine(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}