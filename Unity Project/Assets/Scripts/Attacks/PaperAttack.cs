using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class PaperAttack : Attack
{
	public float DamagePerSecond = 0.75f;


	private Transform tr;


	void Awake()
	{
		tr = transform;
	}
	void Start()
	{
		GetComponent<AudioSource>().PlayOneShot(GameConstants.PaperAttackAudio);
		StartCoroutine(KillAfterTimeCoroutine(GameConstants.PaperAttackAudio.length));
	}

	void FixedUpdate()
	{
		int castLayer = (1 << LayerMask.NameToLayer("Pawn"));

		List<RaycastHit> hits = new List<RaycastHit>();
		Vector3 pos = tr.position;
		hits.AddRange(Physics.RaycastAll(new Ray(pos, new Vector3(-1.0f, 0.0f, 0.0f)),
										 GameRegion.Instance.RegionScale, castLayer));
		hits.AddRange(Physics.RaycastAll(new Ray(pos, new Vector3(1.0f, 0.0f, 0.0f)),
										 GameRegion.Instance.RegionScale, castLayer));
		hits.AddRange(Physics.RaycastAll(new Ray(pos, new Vector3(0.0f, 0.0f, -1.0f)),
										 GameRegion.Instance.RegionScale, castLayer));
		hits.AddRange(Physics.RaycastAll(new Ray(pos, new Vector3(0.0f, 0.0f, 1.0f)),
										 GameRegion.Instance.RegionScale, castLayer));

		float damage = DamagePerSecond * Time.deltaTime;
		foreach (RaycastHit hit in hits)
		{
			Pawn p = hit.collider.GetComponent<Pawn>();
			if (p != null)
			{
				if ((CameFromPlayer && p != Player.Instance) ||
					(!CameFromPlayer && p == Player.Instance))
				{
					p.Damage(damage);
				}
			}
		}
	}
}