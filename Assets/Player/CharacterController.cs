using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Grappable target;

    // References to other components
    Rigidbody2D rb;
    GrappleManager grappleManager;
    DistanceJoint2D joint;
    CharacterPhysics gravity;
    JumpPadPhysics padPhysics;
    RopeAnimation rope;

    [Range(0, 1)]
    public float grabGravityScale;

    public Vector2 position2D => new Vector2(transform.position.x, transform.position.y);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
        grappleManager = FindObjectOfType<GrappleManager>();
        gravity = GetComponent<CharacterPhysics>();
        padPhysics = GetComponent<JumpPadPhysics>();
        rope = GetComponentInChildren<RopeAnimation>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            target = grappleManager.FindClosestGrapplePoint(rb.position);
            switch(target)
			{
                case MovingBlock block:
                    StartCoroutine(GrabSwingPoint(block));
                    break;
                case JumpPad pad:
                    padPhysics.JumpOff(pad);
                    break;
			}
            rope.Attatch(transform, target.transform);
        }
		if (Input.GetKeyUp(KeyCode.Space))
		{
            rope.Deattach();
		}

    }

	private void FixedUpdate()
	{
        if (rb.linearVelocity.y > 0 && IsGrabbed() && !IsAboveGrabbedObject())
            gravity.GravityScale = grabGravityScale;
        else
            gravity.GravityScale = 1;

    }

	private bool IsGrabbed()
	{
		return joint.connectedBody != null && joint.isActiveAndEnabled;
	}

	private bool IsAboveGrabbedObject()
	{
		Vector3 dist = joint.connectedBody.transform.position - transform.position;
		return Vector2.Dot(new Vector2(dist.x, dist.y), Vector2.up) < 0;
	}

    private IEnumerator GrabSwingPoint(MovingBlock point)
	{
        point.Grab(joint);
		do
		{
            yield return new WaitForFixedUpdate();
		} while (Input.GetKey(KeyCode.Space));

        point.Release(joint);
        target = null;
    }
}
