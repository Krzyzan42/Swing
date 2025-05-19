using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float t = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            transform.Translate(0, t, 0);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(-t, 0, 0);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(0, -t, 0);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(t, 0, 0);
    }
}
