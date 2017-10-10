using System;
using System.Collections;
using UnityEngine;

public class ControlState
{
    public bool collideRight { get; set; }
    public bool collideLeft { get; set; }
    public bool collideAbove { get; set; }
    public bool collideBelow { get; set; }
    public bool onGround { get { return collideBelow; } }

    public bool colliding { get { return collideRight || collideLeft || collideAbove || collideBelow; } }

    public void reset()
    {
        collideLeft =
        collideRight =
        collideAbove =
        collideBelow = false;
    }
}