using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    public Transform player;

    public float smoothTime = 0.1f;


    void FixedUpdate()
    {
        Vector2 pos = player.position;
        Vector3 target = new Vector3(player.position.x, player.position.y, -10);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }
}
