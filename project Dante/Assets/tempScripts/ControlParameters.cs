using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class ControlParameters
{
    public Vector2 maxVel = new Vector2(float.MaxValue, float.MaxValue);
    public float gravity = -25f;
    public float maxJumpHeight = 12;
    public float minJumpHeight = 4;
    public float timeBetweenJumps = .25f;

    public int airJumpsAllowed = 2;
}