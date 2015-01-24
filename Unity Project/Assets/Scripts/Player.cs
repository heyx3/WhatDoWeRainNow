using System;
using UnityEngine;


public class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }


	public Transform MyTransform { get; private set; }


	void Awake()
	{
		Instance = this;
		MyTransform = transform;
	}
}