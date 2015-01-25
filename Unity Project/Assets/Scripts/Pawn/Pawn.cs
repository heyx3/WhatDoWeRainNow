using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// An entity that takes actions during his turn.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public abstract class Pawn : MonoBehaviour
{
	public enum WeaponTypes
	{
		Rock,
		Paper,
		Scissors,
	}
	public static Dictionary<WeaponTypes, WeaponTypes> Strengths = new Dictionary<WeaponTypes, WeaponTypes>()
	{
		{ WeaponTypes.Paper, WeaponTypes.Rock },
		{ WeaponTypes.Rock, WeaponTypes.Scissors },
		{ WeaponTypes.Scissors, WeaponTypes.Paper },
	};
	public static Dictionary<WeaponTypes, WeaponTypes> Weaknesses = new Dictionary<WeaponTypes, WeaponTypes>()
	{
		{ WeaponTypes.Paper, WeaponTypes.Scissors },
		{ WeaponTypes.Rock, WeaponTypes.Paper },
		{ WeaponTypes.Scissors, WeaponTypes.Rock },
	};


	public Transform MyTransform { get; private set; }
	public CharacterController Controller { get; private set; }

	/// <summary>
	/// The amount of time left until this pawn can attack again.
	/// </summary>
	public float TimeLeftTillRecharged { get; set; }

	/// <summary>
	/// Starts at 1.0.
	/// </summary>
	public float Health { get; private set; }


	public float RockRechargeTime = 0.5f,
				 PaperRechargeTime = 1.0f,
				 ScissorsRechargeTime = 2.0f;
	public float MoveSpeed = 0.5f;


	private float height = float.NaN;


	public virtual void Damage(float amount)
	{
		Health -= amount;
		if (Health <= 0.0f)
			Destroy(gameObject);
	}


	protected virtual void Awake()
	{
		MyTransform = transform;
		Controller = GetComponent<CharacterController>();
	}
	protected virtual void Update()
	{
		TimeLeftTillRecharged -= Time.deltaTime;
	}
	protected virtual void LateUpdate()
	{
		if (float.IsNaN(height))
		{
			if (Controller.enabled)
				height = MyTransform.position.y + 0.01f;
		}
		else
		{
			Vector3 pos = MyTransform.position;
			MyTransform.position = new Vector3(pos.x, height, pos.z);
		}
	}
	protected virtual void OnDestroy()
	{
		int index = GameRegion.Instance.RoomObj.RoomPawns.IndexOf(this);
		if (index >= 0)
		{
			GameRegion.Instance.RoomObj.RoomPawns.RemoveAt(index);
		}
	}

	protected void Attack(WeaponTypes weapon)
	{
		if (TimeLeftTillRecharged > 0.0f)
			return;

		switch (weapon)
		{
			case WeaponTypes.Rock:
				TimeLeftTillRecharged = RockRechargeTime;
				RockAttack rAttack = ((GameObject)Instantiate(GameConstants.RockPrefab)).GetComponent<RockAttack>();
				rAttack.transform.position = MyTransform.position;
				rAttack.CameFromPlayer = (this == Player.Instance);
				break;

			case WeaponTypes.Paper:
				TimeLeftTillRecharged = PaperRechargeTime;
				//PaperAttack pAttack = ((GameObject)Instantiate(GameConstants.PaperPrefab)).GetComponent<PaperAttack>();
				//pAttack.transform.position = MyTransform.position;
				//pAttack.CameFromPlayer = (this == Player.Instance);
				break;

			case WeaponTypes.Scissors:
				TimeLeftTillRecharged = ScissorsRechargeTime;
				ScissorAttack sAttack = ((GameObject)Instantiate(GameConstants.ScissorsPrefab)).GetComponent<ScissorAttack>();
				sAttack.transform.position = MyTransform.position;
				sAttack.CameFromPlayer = (this == Player.Instance);
				break;

			default: throw new NotImplementedException();
		}
	}
}