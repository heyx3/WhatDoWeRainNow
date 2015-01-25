using System;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class ScissorAttack : Attack
{
	public GameObject BulletPrefab;

	private Transform b1, b2, b3, b4;


	void Start()
	{
		GetComponent<AudioSource>().PlayOneShot(GameConstants.ScissorsAttackAudio);
		StartCoroutine(KillAfterTimeCoroutine(GameConstants.ScissorsAttackAudio.length));

		Vector3 pos = transform.position;
		b1 = ((GameObject)Instantiate(BulletPrefab)).transform;
		b2 = ((GameObject)Instantiate(BulletPrefab)).transform;
		b3 = ((GameObject)Instantiate(BulletPrefab)).transform;
		b4 = ((GameObject)Instantiate(BulletPrefab)).transform;
		b1.position = pos;
		b2.position = pos;
		b3.position = pos;
		b4.position = pos;
		b1.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 45.0f);
		b2.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 135.0f);
		b3.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 225.0f);
		b4.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 315.0f);
	}
}