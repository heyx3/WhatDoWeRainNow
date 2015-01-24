using System;
using UnityEngine;


public class TweenPosition : MonoBehaviour
{
	public Vector3 Target;
	public float MoveTime = 3.0f;


	private Vector3 initial;
	private float t = 0.0f;
	private Transform tr;


	void Awake()
	{
		tr = transform;
		initial = tr.position;
	}

	void Update()
	{
		t += Time.deltaTime;
		if (t >= MoveTime)
		{
			tr.position = Target;
			Destroy(this);
		}
		else
		{
			//Doubly-smooth the movement.
			float smoothedT = Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, t / MoveTime));
			tr.position = Vector3.Lerp(initial, Target, smoothedT);
		}
	}
}