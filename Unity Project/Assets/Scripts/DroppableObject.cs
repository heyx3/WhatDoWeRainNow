using UnityEngine;


public class DroppableObject : MonoBehaviour
{
	public float DropHeightMin = 3.0f,
				 DropHeightMax = 5.0f;

	public float GetDropHeight()
	{
		return Random.Range(DropHeightMin, DropHeightMax);
	}
}