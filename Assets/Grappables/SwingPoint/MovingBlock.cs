using PathCreation;
using System.Collections;
using UnityEngine;

public class MovingBlock : Grappable, IResetable
{
	[System.NonSerialized]
	private new Rigidbody2D rigidbody;

	Coroutine currentRoutine = null;

	public float correctionRange = 0.2f;
	public float correctionStrength = 3f;
	public float speedScale = 20;
	public AnimationCurve speedCurve;
	public PathCreator path;

	public Transform player;

	protected override void Awake()
	{
		base.Awake();
		rigidbody = GetComponent<Rigidbody2D>();
	}

	public void Reset()
	{
		if(currentRoutine != null)
			StopCoroutine(currentRoutine);
	}

	protected override void OnGrab()
	{
		currentRoutine = StartCoroutine(moveCoroutine());
	}

	IEnumerator moveCoroutine()
	{
		float t = 0f;
		while(t < 1f)
		{
			t = path.path.GetClosestTimeOnPath(transform.position);
			Vector3 vel = path.path.GetDirection(t) * speedScale * speedCurve.Evaluate(t);

			float perpendicularness = 1 - Mathf.Abs(Vector3.Dot((player.position - transform.position).normalized, vel.normalized));
			perpendicularness = Mathf.Clamp(perpendicularness, 1 - correctionRange, 1);

			float strength = reframe(perpendicularness, 1-correctionRange, 1, 1 - correctionStrength, 1f);

			rigidbody.linearVelocity = vel * strength;
			yield return null;
		}
		rigidbody.linearVelocity = Vector2.zero;
	}

	float reframe(float x, float startX, float endX, float startY, float endY)
	{
		float percentage = (x - startX) / (endX - startX);
		return startY + percentage * (endY - startY);
	}
}
