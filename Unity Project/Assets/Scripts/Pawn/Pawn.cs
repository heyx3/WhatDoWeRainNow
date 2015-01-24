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


	public float RockRechargeTime = 0.5f,
				 PaperRechargeTime = 1.0f,
				 ScissorsRechargeTime = 2.0f;
	public float MoveSpeed = 0.5f;


	private float height = float.NaN;


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

	protected void Attack(WeaponTypes weapon)
	{
		if (TimeLeftTillRecharged > 0.0f)
			return;

		switch (weapon)
		{
			case WeaponTypes.Rock:
				TimeLeftTillRecharged = RockRechargeTime;
				break;

			case WeaponTypes.Paper:
				TimeLeftTillRecharged = PaperRechargeTime;
				break;

			case WeaponTypes.Scissors:
				TimeLeftTillRecharged = ScissorsRechargeTime;
				break;

			default: throw new NotImplementedException();
		}
	}
}