using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterPhysics : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float fallAccelerationTime;
    [SerializeField] private float maxFallSpeed;
    [Range(0f, 0.1f)]
    [SerializeField] private float fallDrag;

    [Range(0, 0.2f)]
    [SerializeField] private float horizontalDrag = 0.03f;
    [Range(0, 20)]
    [SerializeField] private float maxHorizontalVel = 5;

    public bool EnableGravity { get; set; } = true;
    public float GravityScale { get; set; } = 1f;
    public bool EnableFallDrag { get; set; } = true;
    public float FallDragScale { get; set; } = 1f; 




    private Rigidbody2D rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(EnableFallDrag && rb.linearVelocity.y < -maxFallSpeed)
            ApplyDrag();
        
        if(EnableGravity && rb.linearVelocity.y > -maxFallSpeed)
            ApplyGravity();

        Vector2 velocity = rb.linearVelocity;
        if (Mathf.Abs(velocity.x) > maxHorizontalVel)
            velocity.x *= (1 - horizontalDrag);
        rb.linearVelocity = velocity;
    }

    private void ApplyDrag()
	{
        Vector2 velocity = rb.linearVelocity;
        float speedDiff = Mathf.Abs(velocity.y - maxFallSpeed);

        float slowdown = speedDiff * fallDrag * FallDragScale;
        velocity.y -= slowdown;
        rb.linearVelocity = velocity;
    }

    private void ApplyGravity()
	{
        float downForce = (maxFallSpeed / fallAccelerationTime);
        float downAcceleration = downForce * Time.fixedDeltaTime * GravityScale;

        Vector2 velocity = rb.linearVelocity;
        velocity.y -= downAcceleration;
        if (velocity.y < -maxFallSpeed)
            velocity.y = -maxFallSpeed;

        rb.linearVelocity = velocity;
    }
}
