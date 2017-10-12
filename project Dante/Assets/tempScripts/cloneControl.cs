using System;
using System.Collections;
using UnityEngine;

public class cloneControl : MonoBehaviour
{
    public Vector3
        startPoint,
        endPoint;

    private Player _player;

    public void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void Update()
    {
        startPoint = _player.cloneStart;
        endPoint = _player.cloneFinish;
        transform.position = startPoint;
        transform.Translate(endPoint, Space.World);
    }
}