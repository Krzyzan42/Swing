using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpPad : Grappable
{
	private Rigidbody2D rb;
	public Transform target;

	public Vector2 TargetPosition => new Vector2(target.position.x, target.position.y);
	public Vector2 JumpDirection => new Vector2(transform.up.x, transform.up.y);

	public float distTriggerTreshold = 0.04f;
	public float maxSpeed = 30;
	public AnimationCurve speedCurve;
	public float maxDist = 0;
	public float retreatSpeed = 2f;

	protected override void Awake()
	{
		base.Awake();
		rb = GetComponent<Rigidbody2D>();
	}

	public bool IsTargetReached(Vector2 position)
	{
		return Vector2.Distance(position, TargetPosition) < distTriggerTreshold;
	}

	public void Activate()
	{
		StartCoroutine(Move());
	}

	IEnumerator Move()
	{
		Vector2 initialPosition = transform.position;
		Vector2 direction = transform.up;
		Vector2 currentPosition = transform.position;

		while(Vector2.Distance(initialPosition, currentPosition) < maxDist)
		{
			float t = Vector2.Distance(initialPosition, currentPosition) / maxDist;
			rb.linearVelocity = direction * maxSpeed;

			yield return new WaitForFixedUpdate();
			currentPosition = transform.position;
		}
		rb.linearVelocity = Vector2.zero;

		//return to original position
		rb.linearVelocity = -direction * retreatSpeed;
		while (true)
		{
			if(Vector2.Dot(direction, rb.position - initialPosition) < 0)
			{
				rb.linearVelocity = Vector2.zero;
				rb.position = initialPosition;
				yield break;
			}
			yield return null;
		}
	}
}
