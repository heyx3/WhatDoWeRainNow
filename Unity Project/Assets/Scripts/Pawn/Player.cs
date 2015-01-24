using System;
using UnityEngine;
using GamepadInput;


public class Player : Pawn
{
	public static Player Instance { get; private set; }


	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}
	protected override void Update()
	{
		base.Update();

		if (!Controller.enabled)
			return;


		Vector2 motion = new Vector2(Input.GetAxis("L_XAxis_1"),
									 Input.GetAxis("L_YAxis_1"));
		if (Input.GetKey(KeyCode.A))
			motion.y = 1.0f;
		if (Input.GetKey(KeyCode.D))
			motion.y = -1.0f;
		if (Input.GetKey(KeyCode.S))
			motion.x = -1.0f;
		if (Input.GetKey(KeyCode.W))
			motion.x = 1.0f;
		motion = motion.normalized * MoveSpeed;

		CollisionFlags moveResult = Controller.Move(new Vector3(motion.x, 0.0f, motion.y));

		GamepadState gState = GamePad.GetState(GamePad.Index.Any);
		if (gState.B || Input.GetMouseButton(0))
		{
			Attack(WeaponTypes.Rock);
		}
		else if (gState.Y || Input.GetMouseButton(1))
		{
			Attack(WeaponTypes.Paper);
		}
		else if (gState.X || Input.GetMouseButton(2))
		{
			Attack(WeaponTypes.Scissors);
		}
	}
}