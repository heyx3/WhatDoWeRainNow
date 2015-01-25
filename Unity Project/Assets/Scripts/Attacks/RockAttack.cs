using System;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class RockAttack : Attack
{
	public float KillAfterTime = 1.499f;
	public float Damage = 0.4f;


	void Start()
	{
		GetComponent<AudioSource>().PlayOneShot(GameConstants.RockAttackAudio);


		Vector2 myPos = Horizontal(transform.position);
		float distSqr = GameConstants.RockAttackDist * GameConstants.RockAttackDist;

		if (CameFromPlayer)
		{
			foreach (Pawn enemy in GameRegion.Instance.RoomObj.RoomPawns)
				if (distSqr >= (Horizontal(enemy.MyTransform.position) - myPos).sqrMagnitude)
					enemy.Damage(Damage);
		}
		else
		{
			if (distSqr >= (Horizontal(Player.Instance.MyTransform.position) - myPos).sqrMagnitude)
				Player.Instance.Damage(Damage);
		}

		StartCoroutine(KillAfterTimeCoroutine(KillAfterTime));
	}


	void OnDrawGizmos()
	{
		GameConstants consts = FindObjectOfType<GameConstants>();
		if (consts == null) return;

		Gizmos.color = new Color(0.4f, 0.05f, 0.05f, 0.3f);
		Gizmos.DrawSphere(transform.position, consts._RockAttackDist);
	}
}