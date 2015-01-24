using System;
using UnityEngine;


public abstract class RiftUIElement : MonoBehaviour
{
	public Transform MyTransform { get; private set; }


	/// <summary>
	/// Called when this object is selected by the player.
	/// </summary>
	public abstract void Selected();


	protected virtual void Awake()
	{
		MyTransform = transform;
	}
}