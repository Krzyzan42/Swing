using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadPhysics : MonoBehaviour
{
    private Vector2 savedVelocity;

    Rigidbody2D rb;

    public AnimationCurve jumppadSpeedChange;
    public float maxApproachSpeed = 20;
    public float velocitySaveDist = 1f;

    public Vector2 position2D => new Vector2(transform.position.x, transform.position.y);


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void JumpOff(JumpPad pad)
	{
        StartCoroutine(JumpOffCoroutine(pad));
	}

    private IEnumerator JumpOffCoroutine(JumpPad pad)
    {
        float t = 0;
        Vector2 initial = rb.linearVelocity;
        savedVelocity = Vector2.zero;

        while (t < 1 && !pad.IsTargetReached(rb.position))
        {
            Vector2 dir = (pad.TargetPosition - position2D).normalized;
            Vector2 vel = Vector2.Lerp(initial, dir * maxApproachSpeed, jumppadSpeedChange.Evaluate(t));

            if(Vector2.Distance(pad.TargetPosition, position2D) < velocitySaveDist && savedVelocity == Vector2.zero)
			{
                Vector2 perp = Vector2.Perpendicular(pad.JumpDirection).normalized;
                savedVelocity = Vector2.Dot(perp, vel) * perp;
			}
            rb.linearVelocity = vel;
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log(savedVelocity);
        pad.Activate();
        rb.linearVelocity = pad.maxSpeed * pad.JumpDirection + savedVelocity;
    }
}
