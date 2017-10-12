using System;
using System.Collections;
using UnityEngine;

public class CharControl : MonoBehaviour
{
    private const float skin = .02f;
    private const int numHorRays = 8;
    private const int numVertRays = 4;

    public LayerMask layerMask;
    public LayerMask cloneMask;
    public ControlParameters parameters;
    public ControlState state;
    public bool colliding { get; set; }
    public Vector2 velocity { get { return _velocity; } }
    public GameObject underFoot { get; set; }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _collider;
    private GameObject lastUnderFoot;
    private float jumpTimer;

    private Vector3
        rayOriginTL,
        rayOriginBR,
        rayOriginBL;

    private float
        vertRayGap,
        horRayGap;

    public void Awake()
    {
        colliding = true;
        state = new ControlState();
        _transform = transform;
        _localScale = transform.localScale;
        _collider = GetComponent<BoxCollider2D>();

        var collisionWidth = _collider.size.x * Mathf.Abs(transform.localScale.x) - (2 * skin);
        horRayGap = collisionWidth / (numVertRays - 1);

        var collisionHeight = _collider.size.y * Mathf.Abs(transform.localScale.y) - (2 * skin);
        vertRayGap = collisionHeight / (numHorRays - 1);
    }

    public bool canJump()
    {
        if (state.onGround)
            return true;
        return false;
    }

    public void addForce(Vector2 force)
    {
        _velocity += force;
    }

    public void setForce(Vector2 force)
    {
        _velocity = force;
    }

    public void setHorizonForce(float x)
    {
        _velocity.x = x;
    }

    public void setVertForce(float y)
    {
        _velocity.y = y;
    }

    public void jump()
    {
        addForce(new Vector2(0, parameters.jumpHeight));
        jumpTimer = parameters.timeBetweenJumps;
    }

    public void LateUpdate()
    {
        jumpTimer -= Time.deltaTime;

        _velocity.y += parameters.gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);
    }

    private void Move(Vector2 moveChange)
    {
        var onGround = state.collideBelow;
        state.reset();

        if (colliding)
        {
            calcRayOrigin();
            if (Mathf.Abs(moveChange.x) > .001f)
                moveHorizon(ref moveChange);
            moveVertical(ref moveChange);
        }

        transform.Translate(moveChange, Space.World);

        if(Time.deltaTime > 0)
            _velocity = moveChange / Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, parameters.maxVel.x);
        _velocity.y = Mathf.Min(_velocity.y, parameters.maxVel.y);
    }

    private void calcRayOrigin()
    {
        var colliderSize = new Vector2(_collider.size.x * Mathf.Abs(_localScale.x), _collider.size.y * Mathf.Abs(_localScale.y)) / 2;
        var center = new Vector2(_collider.offset.x * _localScale.x, _collider.offset.y * _localScale.y);

        rayOriginTL = _transform.position + new Vector3(center.x - colliderSize.x + skin, center.y + colliderSize.y - skin);
        rayOriginBR = _transform.position + new Vector3(center.x + colliderSize.x - skin, center.y - colliderSize.y + skin);
        rayOriginBL = _transform.position + new Vector3(center.x - colliderSize.x + skin, center.y - colliderSize.y + skin);

    }

    private void moveVertical(ref Vector2 moveChange)
    {
        var goingUp = moveChange.y > 0;
        var rayLength = Mathf.Abs(moveChange.y) + skin;
        var rayPoint = goingUp ? Vector2.up : -Vector2.up;
        var rayOrigin = goingUp ? rayOriginTL : rayOriginBL;

        rayOrigin.x += moveChange.x;

        var distanceToGround = float.MaxValue;
        for(var i = 0; i < numVertRays; i++)
        {
            var ray = new Vector2(rayOrigin.x + (i * horRayGap), rayOrigin.y);
            var rayCastHit = Physics2D.Raycast(ray, rayPoint, rayLength, layerMask | cloneMask);
            Debug.DrawRay(ray, rayPoint * rayLength, Color.red);
            if (!rayCastHit)
                continue;

            if(!goingUp)
            {
                var distanceToHit = _transform.position.y - rayCastHit.point.y;
                if(distanceToHit < distanceToGround)
                {
                    distanceToGround = distanceToHit;
                    underFoot = rayCastHit.collider.gameObject;
                }
            }

            moveChange.y = rayCastHit.point.y - ray.y;
            rayLength = Mathf.Abs(moveChange.y);
            if (goingUp)
            {
                moveChange.y -= skin;
                state.collideAbove = true;
            }
            else
            {
                moveChange.y += skin;
                state.collideBelow = true;
            }

            if (rayLength < skin + .0001f)
                break;
        }
    }

    private void moveHorizon(ref Vector2 moveChange)
    {
        var goingRight = moveChange.x > 0;
        var rayLength = Mathf.Abs(moveChange.x) + skin;
        var rayPoint = goingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = goingRight ? rayOriginBR : rayOriginBL;

        for(var i = 0; i < numHorRays; i++)
        {
            var ray = new Vector2(rayOrigin.x, rayOrigin.y + (i * vertRayGap));

            var rayCastHit = Physics2D.Raycast(ray, rayPoint, rayLength, layerMask | cloneMask);
            if (!rayCastHit)
                continue;

            moveChange.x = rayCastHit.point.x - ray.x;
            rayLength = Mathf.Abs(moveChange.x);

            if(goingRight)
            {
                moveChange.x -= skin;
                state.collideRight = true;
            }
            else
            {
                moveChange.x += skin;
                state.collideLeft = true;
            }

            if (rayLength < skin + .0001f)
                break;
        }
    }
}