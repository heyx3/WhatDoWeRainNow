using System;
using UnityEngine;


/// <summary>
/// Drops an object onto the floor, making it bounce up and down for a bit.
/// This component deletes itself when finished.
/// After the bouncing is finished, removes this object's Rigidbody/DroppableObject
///     and disables its collider.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(DroppableObject))]
[RequireComponent(typeof(Rigidbody))]
public class ObjectBounce : MonoBehaviour
{
	public float BounceMaxVelocity = 0.01f;
	public float NoBounceTime = 1.0f;


	private Rigidbody rgd;
	private float timeNotBouncing = -1.0f;


	void Awake()
	{
		rgd = gameObject.GetComponent<Rigidbody>();
		rgd.constraints = RigidbodyConstraints.FreezeRotation |
						  RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

		Vector3 pos = rgd.position;
		rgd.position = new Vector3(pos.x, GetComponent<DroppableObject>().DropHeight, pos.z);
	}
	void OnDestroy()
	{
		Destroy(rgd);
		Destroy(GetComponent<DroppableObject>());
		GetComponent<Collider>().enabled = false;
	}

	void FixedUpdate()
	{
		if (rgd.velocity.magnitude < BounceMaxVelocity)
		{
			if (timeNotBouncing > NoBounceTime)
			{
				Destroy(this);
			}
			else if (timeNotBouncing < 0.0f)
			{
				timeNotBouncing = 0.0f;
			}
			else
			{
				timeNotBouncing += Time.deltaTime;
			}
		}
		else
		{
			timeNotBouncing = -1.0f;
		}
	}
}