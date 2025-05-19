using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Grappable : MonoBehaviour
{
	new Rigidbody2D rigidbody;
    GrappleManager grappleManager;

    public Vector2 position2D => new Vector2(transform.position.x, transform.position.y);

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
	{
        grappleManager = FindObjectOfType<GrappleManager>();
        grappleManager.AddGrappable(this);
	}

    public void Grab(Joint2D joint)
	{
        joint.connectedBody = rigidbody;
        joint.enabled = true;

        OnGrab();
	}

    public void Release(Joint2D joint)
	{
        joint.enabled = false;

        OnRelease();
	}

    protected virtual void OnGrab() { }

    protected virtual void OnRelease() { }
}
