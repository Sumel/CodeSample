using UnityEngine;
using System.Collections;
using System;

public class CharacterController2D : MonoBehaviour
{
    private const float SkinWidth = .02f;
    private const int TotalHorizontalRays = 8;
    private const int TotalVerticalRays = 4;

    private static readonly float SlopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);

    public LayerMask PlatformMask;
    public ControllerParameters2D DefaultParameters;

    public ControllerState2D State { get; private set; }

    public Vector2 Velocity { get { return _velocity; } }


    private bool _gravityEnabled = true;

    public bool GravityEnabled
    {
        get { return _gravityEnabled; }
        set { _gravityEnabled = value; }
    }

    public bool CanEverJump
    {
        get { return Parameters.JumpRestrictions != ControllerParameters2D.JumpBehavior.CantJump; }
    }

    public bool CanJump
    {
        get
        {
            if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpAnywhere)
                return _jumpIn < 0;
            if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpOnGround && !IsOnSteepSlope)
                return State.IsGrounded;
            return false;
        }
    }

    public bool IsOnSteepSlope
    {
        get
        {
            if (!State.IsGrounded || StandingOn == null)
            {
                return false;
            }

            float rayDistance = 0.2f + SkinWidth;
            Vector2 rayDirection = -Vector2.up;
            Vector2 rayOrigin = _raycastBottomLeft;
            RaycastHit2D goodResult = default(RaycastHit2D);
            bool foundSomething = false;
            for (int i = 0; i < TotalVerticalRays; i++)
            {
                Vector2 rayVector = new Vector2(rayOrigin.x + i * _horizontalDistanceBetweenRays, rayOrigin.y);

                RaycastHit2D[] allResults = Physics2D.RaycastAll(rayVector, rayDirection, rayDistance, PlatformMask);
                if (allResults == null)
                    continue;
                foreach (RaycastHit2D rayHit in allResults)
                {
                    if (rayHit.collider.gameObject == StandingOn)
                    {
                        goodResult = rayHit;
                        foundSomething = true;
                        break;
                    }
                }
                if (foundSomething)
                    break;
            }
            if (!foundSomething)
                return false;
            float angle = Vector2.Angle(goodResult.normal, Vector2.up);
            if (angle <= Parameters.SlopeJumpLimit)
            {
                return false;
            }

            return true;
        }
    }

    public bool HandleCollisions { get; set; }
    public ControllerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }
    public GameObject StandingOn { get; private set; }
    public Vector3 PlatformVelocity { get; private set; }

    private Vector2 _velocity;
    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _boxCollider;
    private ControllerParameters2D _overrideParameters = null;

    private float _jumpIn;
    private GameObject _lastStandingOn;

    private Vector3
        _activeGlobalPlatformPoint,
        _activeLocalPlatformPoint;

    private Vector3
        _raycastTopLeft,
        _raycastBottomLeft,
        _raycastBottomRight;

    private float
        _verticalDistanceBetweenRays,
        _horizontalDistanceBetweenRays;


    public void Awake()
    {
        HandleCollisions = true;
        State = new ControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        float colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - 2 * SkinWidth;
        float colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - 2 * SkinWidth;
        _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
        _horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

    }

    public void AddForce(Vector2 force)
    {
        _velocity += force;
    }

    public void SetForce(Vector2 force)
    {
        _velocity = force;
    }

    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }

    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }

    public void TryJump()
    {
        if (CanJump)
        {
            Jump();
        }
    }

    public void Jump()
    {
        AddForce(new Vector2(0, Parameters.JumpMagnitude));
        _jumpIn = Parameters.JumpFrequency;
    }
    public void LateUpdate()
    {
        float startVelx = _velocity.x;
        _jumpIn -= Time.deltaTime;
        if (GravityEnabled)
        {
            _velocity.y += Parameters.Gravity * Time.deltaTime;
        }
        LimitVelocity();
        Move(Velocity * Time.deltaTime);
        float endVelX = _velocity.x;

    }

    private void Teleport(Vector2 position)
    {
        transform.position = position;
        CalculateRayOrigins();
    }

    private void Move(Vector2 deltaMovement)
    {
        bool wasGrounded = State.IsCollidingBelow;
        State.Reset();

        if (HandleCollisions)
        {
            HandlePlatforms();
            CalculateRayOrigins();

            if (deltaMovement.y < 0 && wasGrounded)
            {
                HandleVerticalSlope(ref deltaMovement);
            }
            if (Mathf.Abs(deltaMovement.x) > .0001f)
            {
                MoveHorizontally(ref deltaMovement);
            }

            MoveVertically(ref deltaMovement);

            CorrectHorizontalPlacement(ref deltaMovement, true);
            CorrectHorizontalPlacement(ref deltaMovement, false);
        }

        _transform.Translate(deltaMovement, Space.World);

        if (Time.deltaTime > 0)
        {
            _velocity = deltaMovement / Time.deltaTime;
        }

        if (State.IsMovingUpSlope)
        {
            _velocity.y = 0;
        }

        if (StandingOn != null)
        {
            _activeGlobalPlatformPoint = transform.position;
            _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);

            if (_lastStandingOn != StandingOn)
            {
                if (_lastStandingOn != null)
                {
                    _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
                }
                StandingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
                _lastStandingOn = StandingOn;
            }
            else
            {
                StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (_lastStandingOn != null)
        {
            _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
            _lastStandingOn = null;
        }
    }

    private void LimitVelocity()
    {
        int XMultiplier = 1;
        int YMultiplier = 1;
        if (_velocity.x < 0)
        {
            XMultiplier = -1;
        }
        if (_velocity.y < 0)
        {
            YMultiplier = -1;
        }
        _velocity.x = Mathf.Min(Math.Abs(_velocity.x), Parameters.MaxVelocity.x) * XMultiplier;
        _velocity.y = Mathf.Min(Math.Abs(_velocity.y), Parameters.MaxVelocity.y) * YMultiplier;
    }

    private void HandlePlatforms()
    {
        if (StandingOn != null)
        {
            Vector3 newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
            Vector3 moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

            if (moveDistance != Vector3.zero)
            {
                transform.Translate(moveDistance, Space.World);
            }

            PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
        }
        else
        {
            PlatformVelocity = Vector3.zero;
        }
        StandingOn = null;
    }

    private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
    {
        float halfWidth = _boxCollider.size.x * _localScale.x / 2f;
        Vector3 rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;
        if (isRight)
        {
            rayOrigin.x -= (halfWidth - SkinWidth);
        }
        else
        {
            rayOrigin.x += (halfWidth - SkinWidth);
        }
        rayOrigin.x = _transform.position.x;
        Vector2 rayDirection = isRight ? Vector2.right : -Vector2.right;
        float offset = 0f;
        for (int i = 1; i < TotalHorizontalRays - 1; i++)
        {
            Vector2 rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));



            RaycastHit2D raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
            if (!raycastHit)
                continue;
            offset = isRight ? ((raycastHit.point.x - (_transform.position.x + deltaMovement.x)) - halfWidth) : (halfWidth - (_transform.position.x + deltaMovement.x - raycastHit.point.x));
        }
        deltaMovement.x += offset;
    }

    private void CalculateRayOrigins()
    {
        Vector2 size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        Vector2 center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        _raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        _raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);

    }

    private void MoveHorizontally(ref Vector2 deltaMovement)
    {
        bool isGoingRight = deltaMovement.x > 0;
        float rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        Vector2 rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        Vector2 rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

        float movX = deltaMovement.x;
        for (int i = 0; i < TotalHorizontalRays; i++)
        {
            Vector2 rayVector = new Vector2(rayOrigin.x, rayOrigin.y + i * _verticalDistanceBetweenRays);

            RaycastHit2D rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            if (!rayCastHit)
            {
                continue;
            }

            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, rayCastHit, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
            {
                break;
            }

            if (i * _verticalDistanceBetweenRays <= Parameters.MaxIgnoredHeight)
            {
                if (HandleSmallObstacles(ref deltaMovement, rayCastHit, isGoingRight))
                {
                    continue;
                }
            }

            deltaMovement.x = rayCastHit.point.x - rayVector.x;
            rayDistance = Mathf.Abs(deltaMovement.x);

            if (isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;
            }
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }

            if (rayDistance < SkinWidth + .0001f)
            {
                break;
            }

        }

    }

    private void MoveVertically(ref Vector2 deltaMovement)
    {

        bool isGoingUp = deltaMovement.y > 0;
        float rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        Vector2 rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
        Vector2 rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;

        rayOrigin.x += deltaMovement.x;

        float standingOnDistance = float.MaxValue;
        for (int i = 0; i < TotalVerticalRays; i++)
        {
            Vector2 rayVector = new Vector2(rayOrigin.x + i * _horizontalDistanceBetweenRays, rayOrigin.y);
            //Debug.DrawRay(rayVector,rayDirection * rayDistance,Color.red);

            RaycastHit2D rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            if (!rayCastHit)
            {
                continue;
            }

            if (!isGoingUp)
            {
                float verticalDistanceToHit = _transform.position.y - rayCastHit.point.y;
                if (verticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = verticalDistanceToHit;
                    StandingOn = rayCastHit.collider.gameObject;
                }
            }

            deltaMovement.y = rayCastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isGoingUp)
            {
                deltaMovement.y -= SkinWidth;
                State.IsCollidingAbove = true;
            }
            else
            {
                deltaMovement.y += SkinWidth;
                State.IsCollidingBelow = true;
            }

            if (!isGoingUp && deltaMovement.y > .0001f)
            {
                State.IsMovingUpSlope = true;
            }

            if (rayDistance < SkinWidth + .0001f)
            {
                break;
            }

        }
    }

    public bool HandleSmallObstacles(ref Vector2 deltaMovement, RaycastHit2D oldRayHit, bool isGoingRight)
    {
        Vector2 rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;
        Vector2 rayDir = isGoingRight ? Vector2.right : -Vector2.right;
        rayOrigin.y += Parameters.MaxIgnoredHeight;
        float rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        RaycastHit2D rayHit = Physics2D.Raycast(rayOrigin, rayDir, rayDistance, PlatformMask);
        if (rayHit == true)
            return false;

        //checking if it's not actually a slope
        rayDistance = SkinWidth + Parameters.MaxIgnoredHeight / (Mathf.Tan(Mathf.Deg2Rad * Parameters.SlopeLimit));
        RaycastHit2D[] moreRays = Physics2D.RaycastAll(rayOrigin, rayDir, rayDistance, PlatformMask);
        if (moreRays != null)
        {
            foreach (RaycastHit2D rh in moreRays)
            {
                if (rh.collider.gameObject == oldRayHit.collider.gameObject)
                {
                    return false;
                }
            }
        }

        rayDistance = Parameters.MaxIgnoredHeight;
        Vector2 rayOr2 = oldRayHit.point;
        rayOr2.y += Parameters.MaxIgnoredHeight;
        RaycastHit2D anotherRayHit = Physics2D.Raycast(rayOr2, -Vector2.up, rayDistance, PlatformMask);
        if (!anotherRayHit)
        {
            //something probably went kinda wrong so we dont try to correct y position
            return true;
        }
        float yDistance = anotherRayHit.point.y - (_raycastBottomLeft.y + SkinWidth);

        Vector2 newPos = transform.position;
        newPos.y += yDistance;

        Teleport(newPos);

        return true;
    }

    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
        float center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
        Vector2 direction = -Vector2.up;

        float slopeDistance = SlopeLimitTangent * (_raycastBottomRight.x - center);
        Vector2 slopeRayVector = new Vector2(center, _raycastBottomLeft.y);

        RaycastHit2D raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        if (!raycastHit)
            return;

        bool isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
        if (!isMovingDownSlope)
            return;
        float angle = Vector2.Angle(raycastHit.normal, Vector2.up);
        if (Mathf.Abs(angle) < .0001f)
            return;

        State.IsMovingDownSlope = true;
        State.SlopeAngle = angle;
        deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
    }

    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, RaycastHit2D oldRayHit, float angle, bool isGoingRight)
    {
        if (Mathf.RoundToInt(angle) == 90)
        {
            return false;
        }
        if (angle > Parameters.SlopeLimit)
        {
            if (!HandleSmallObstacles(ref deltaMovement, oldRayHit, isGoingRight))
                deltaMovement.x = 0;
            return true;
        }
        if (deltaMovement.y > .07f)
            return true;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
        State.IsMovingUpSlope = true;
        State.IsCollidingBelow = true;
        return true;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        ControllerPhysicsVolume2D parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
        if (parameters == null)
            return;
        _overrideParameters = parameters.Parameters;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        ControllerPhysicsVolume2D parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
        if (parameters == null)
            return;

        _overrideParameters = null;
    }
}
