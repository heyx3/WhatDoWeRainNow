using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class ScissorBullet : MonoBehaviour
{
	[NonSerialized]
	public bool CameFromPlayer;

	public GameObject DestroyParticlesChild;
	public float DestroyParticlesKillTime = 1.0f;

	public float Speed = 0.05f;
	public float Damage = 0.5f;


	private Rigidbody rgd;
	private Transform tr;

	private Vector3 GetMovementDir() { return tr.up; }


	void Awake()
	{
		rgd = rigidbody;
		tr = transform;

		if (!collider.isTrigger)
			Debug.LogError("Bullets need to use trigger colliders!");
		if (rgd.isKinematic)
			Debug.LogError("Bullets shouldn't be kinematic rigidbodies!");
		
		DestroyParticlesChild.SetActive(false);
	}
	void FixedUpdate()
	{
		rgd.velocity = GetMovementDir() * Speed;
	}
	void OnTriggerEnter(Collider other)
	{
		//Ignore collisions with other bullets.
		if (other.GetComponent<ScissorBullet>() != null)
			return;

		Pawn pawn = other.GetComponent<Pawn>();
		if (pawn != null)
		{
			//Don't do anything if this is the entity that fired the bullet.
			if ((CameFromPlayer && pawn == Player.Instance) ||
				(!CameFromPlayer && pawn != Player.Instance))
			{
				return;
			}

			//Damage the entity.
			pawn.Damage(Damage);
		}


		//Hide this entity and start the smoke effects.
		renderer.enabled = false;
		DestroyParticlesChild.SetActive(true);
		StartCoroutine(KillAfterTime());
	}

	void OnDrawGizmos()
	{
		tr = transform;
		Gizmos.color = Color.white;
		Gizmos.DrawRay(tr.position, GetMovementDir());
	}


	private System.Collections.IEnumerator KillAfterTime()
	{
		yield return new WaitForSeconds(DestroyParticlesKillTime);
		Destroy(gameObject);
	}
}