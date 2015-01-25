using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class HealthBar : MonoBehaviour
{
	public float Alpha = 0.45f;


	private Transform tr;
	private Renderer rend;
	public Pawn parent;
	private float originalScale;


	void Awake()
	{
		tr = transform;
		rend = renderer;
		parent = tr.parent.GetComponent<Pawn>();
		originalScale = tr.localScale.x;
	}
	void Update()
	{
		tr.localScale = new Vector3(Mathf.Lerp(0.0f, originalScale, parent.Health),
									tr.localScale.y, tr.localScale.z);
		
		Vector3 col = Vector3.Lerp(new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f),
								   parent.Health);
		rend.material.color = new Color(col.x, col.y, col.z, Alpha);

		Vector3 lookDir = (tr.position - PlayerCamera.Instance.FaceTracker.position);
		lookDir.y = 0.0f;
		tr.forward = lookDir.normalized;
	}
}