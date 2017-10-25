using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CamControl : MonoBehaviour
{
    public Transform player;
    public Vector2 buffer, smoothing;
    public BoxCollider2D levelBound;

    private Vector3 lowerBound, upperBound;

    public bool followPlayer { get; set; }

    public void Start()
    {
        lowerBound = levelBound.bounds.min;
        upperBound = levelBound.bounds.max;
        followPlayer = true;
    }

    public void Update()
    {
        var camx = transform.position.x;
        var camy = transform.position.y;

        if (followPlayer)
        {
            if (Mathf.Abs(camx - player.position.x) > buffer.x)
                camx = Mathf.Lerp(camx, player.position.x, smoothing.x * Time.deltaTime);
            if (Mathf.Abs(camy - player.position.y) > buffer.y)
                camy = Mathf.Lerp(camy, player.position.y, smoothing.y * Time.deltaTime);
        }

        var camRadius = Camera.main.orthographicSize * ((float) Screen.width / Screen.height);

        camx = Mathf.Clamp(camx, lowerBound.x + camRadius, upperBound.x - camRadius);
        camy = Mathf.Clamp(camy, lowerBound.y + Camera.main.orthographicSize, upperBound.y - Camera.main.orthographicSize);

        transform.position = new Vector3(camx, camy, transform.position.z);
    }
}
