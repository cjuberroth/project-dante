using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool facingRight;
    private CharControl controller;
    private float normalizedSpeed;

    public float maxSpeed = 8;
    public float groundAccel = 10f;
    public float airAccel = 5f;
    
    public void Awake()
    {
        controller = GetComponent<CharControl>();
        facingRight = transform.localScale.x > 0;
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
    }

    private void flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = transform.localScale.x > 0;
    }
}