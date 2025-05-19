using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    Vector2 initialPosition;
    Quaternion initialRotation;
    Vector2 initialVelocity;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            initialVelocity = rb.linearVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.R))
		{
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            if(rb != null)
                rb.linearVelocity = initialVelocity;

            var resetableComponents = GetComponents<IResetable>();
			foreach (var resetable in resetableComponents)
			{
                resetable.Reset();
			}
		}
    }
}
