using System;
using System.Linq;
using UnityEngine;


/// <summary>
/// Drops an object onto the floor, making it bounce up and down for a bit.
/// This component deletes itself when finished.
/// After the bouncing is finished, removes this object's Rigidbody/DroppableObject
///     and enables the character controller component if it exists.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(DroppableObject))]
[RequireComponent(typeof(Rigidbody))]
public class ObjectBounce : MonoBehaviour
{
	public float BounceMaxVelocity = 0.01f;
	public float NoBounceTime = 1.0f;


	private Rigidbody rgd;
	private Collider coll;
	private CharacterController contr;

	private float timeNotBouncing = -1.0f;


	void Awake()
	{
		rgd = gameObject.GetComponent<Rigidbody>();
		contr = GetComponent<CharacterController>();
		coll = GetComponents<Collider>().Where(c => (c != contr)).First();

		rgd.constraints = RigidbodyConstraints.FreezeRotation |
						  RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		if (contr != null)
			contr.enabled = false;
	}
	void Start()
	{
		if (contr != null)
			contr.enabled = false;

		Vector3 pos = rgd.position;
		rgd.transform.position = new Vector3(pos.x, GetComponent<DroppableObject>().GetDropHeight(), pos.z);
	}
	void OnDestroy()
	{
		Destroy(rgd);
		Destroy(GetComponent<DroppableObject>());
		if (contr != null)
		{
			Destroy(coll);
			contr.enabled = true;
		}
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