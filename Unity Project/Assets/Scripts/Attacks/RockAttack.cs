using System;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class RockAttack : MonoBehaviour
{
	/// <summary>
	/// Gets the horizontal component of the given vector.
	/// </summary>
	private static Vector2 Horizontal(Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	public bool CameFromPlayer;

	void Start()
	{
		GetComponent<AudioSource>().PlayOneShot(GameConstants.RockAttackAudio);

		Vector2 myPos = Horizontal(transform.position);
		float distSqr = GameConstants.RockAttackDist * GameConstants.RockAttackDist;

		if (CameFromPlayer)
		{
			foreach (Pawn enemy in GameRegion.Instance.RoomObj.RoomPawns)
				if (distSqr >= (Horizontal(enemy.MyTransform.position) - myPos).sqrMagnitude)
					enemy.Damage(GameConstants.RockAttackDamage);
		}
		else
		{
			if (distSqr >= (Horizontal(Player.Instance.MyTransform.position) - myPos).sqrMagnitude)
				Player.Instance.Damage(GameConstants.RockAttackDamage);
		}
	}


	void OnDrawGizmos()
	{
		GameConstants consts = FindObjectOfType<GameConstants>();
		if (consts == null) return;

		Gizmos.color = new Color(0.4f, 0.05f, 0.05f, 0.1f);
		Gizmos.DrawSphere(transform.position, consts._RockAttackDist);
	}
}