using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool facingRight;
    private CharControl controller;
    private float normalizedSpeed;
    private ControlState state;

    public float maxSpeed = 8;
    public float groundAccel = 10f;
    public float airAccel = 5f;

    public Vector3
        cloneStart,
        cloneFinish;

    public Transform[] clonePath;

    public void Awake()
    {
        controller = GetComponent<CharControl>();
        facingRight = transform.localScale.x > 0;
        cloneStart = Vector3.zero;
        cloneFinish = Vector3.zero;
    }

    public void Update()
    {
        handleInput();
        var moveFactor = controller.state.onGround ? groundAccel : airAccel;
        controller.setHorizonForce(Mathf.Lerp(controller.velocity.x, normalizedSpeed * maxSpeed, Time.deltaTime * moveFactor));
    }

    private void handleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            normalizedSpeed = 1;
            if (!facingRight)
                flip();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            normalizedSpeed = -1;
            if (facingRight)
                flip();
        }
        else
        {
            normalizedSpeed = 0;
        }

        if(controller.canJump())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.parameters.airJumpsAllowed--;
                controller.jump();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
            controller.stopJump();

    }

    private void flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = transform.localScale.x > 0;
    }
}