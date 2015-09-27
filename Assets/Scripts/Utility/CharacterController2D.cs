#define DEBUG_CC2D_RAYS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[AddComponentMenu("Utility/Character Controller 2D")]
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
  private struct CharacterRaycastOrigins
  {
    public Vector3 topLeft;
    public Vector3 bottomRight;
    public Vector3 bottomLeft;
  }

  public class CharacterCollisionState2D
  {
    public bool Right { get; set; }
    public bool Left { get; set; }
    public bool Above { get; set; }
    public bool Below { get; set; }
    public bool BecameGroundedThisFrame { get; set; }
    public bool WasGroundedLastFrame { get; set; }
    public bool MovingDownSlope { get; set; }
    public float SlopeAngle { get; set; }

    public bool HasCollision
      => Below || Right || Left || Above;

    public void Reset()
    {
      Right = Left = Above = Below = BecameGroundedThisFrame = MovingDownSlope = false;
      SlopeAngle = 0f;
    }

    public override string ToString()
      => $"[CharacterCollisionState2D] r: {Right}, l: {Left}, a: {Above}, b: {Below}, movingDownSlope: {MovingDownSlope}, angle: {SlopeAngle}, wasGroundedLastFrame: {WasGroundedLastFrame}, becameGroundedThisFrame: {BecameGroundedThisFrame}";
  }

  public event Action<RaycastHit2D> ControllerCollided;
  public event Action<Collider2D> TriggerEnter;
  public event Action<Collider2D> TriggerStay;
  public event Action<Collider2D> TriggerExit;

  [SerializeField, Range(0.001f, 0.3f)] private float skinWidth = 0.02f;
  [SerializeField] private LayerMask platformMask = 0;
  [SerializeField] private LayerMask oneWayPlatformMask = 0;
  [SerializeField] private float jumpingThreshold = 0.07f;
  [SerializeField] private AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1.5f), new Keyframe(0, 1), new Keyframe(90, 0));
  [SerializeField, Range(0, 90f)] private float slopeLimit = 30f;
  [SerializeField, Range(2, 20)] private int totalHorizontalRays = 8;
  [SerializeField, Range(2, 20)] private int totalVerticalRays = 4;

  private CharacterRaycastOrigins raycastOrigins;
  private RaycastHit2D raycastHit;
  private List<RaycastHit2D> raycastHitsThisFrame = new List<RaycastHit2D>(2);
  private float verticalDistanceBetweenRays;
  private float horizontalDistanceBetweenRays;
  private bool isGoingUpSlope;

  private CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
  private BoxCollider2D boxCollider;
  private Vector3 velocity;

  private float SlopeLimitTangent { get; } = Mathf.Tan(75f * Mathf.Deg2Rad);
  private BoxCollider2D BoxCollider => this.GetComponentIfNull(ref this.boxCollider);

  public Vector3 Velocity => this.velocity;
  public bool IsGrounded => this.collisionState.Below;
  public bool WasGroundedLastFrame => this.collisionState.WasGroundedLastFrame;
  public LayerMask PlatformMask => this.platformMask;

  public float SkinWidth
  {
    get { return this.skinWidth; }
    set
    {
      this.skinWidth = value;
      RecalculateDistanceBetweenRays();
    }
  }

  private void Awake()
  {
    this.platformMask = this.platformMask | this.oneWayPlatformMask;
    RecalculateDistanceBetweenRays();
  }

  private void OnTriggerEnter2D(Collider2D col)
    => TriggerEnter?.Invoke(col);

  private void OnTriggerStay2D(Collider2D col)
    => TriggerStay?.Invoke(col);

  private void OnTriggerExit2D(Collider2D col)
    => TriggerExit?.Invoke(col);

  [Conditional("DEBUG_CC2D_RAYS")]
  private static void DrawRay(Vector3 start, Vector3 dir, Color color)
    => Debug.DrawRay(start, dir, color);

  public void Move(Vector3 deltaMovement)
  {
    if (Time.deltaTime <= 0f || Time.timeScale <= 0.01f)
      return;

    this.collisionState.WasGroundedLastFrame = this.collisionState.Below;
    this.collisionState.Reset();
    this.raycastHitsThisFrame.Clear();
    this.isGoingUpSlope = false;

    PrimeRaycastOrigins();

    if (deltaMovement.y < 0f && this.collisionState.WasGroundedLastFrame)
      HandleVerticalSlope(ref deltaMovement);

    if (Math.Abs(deltaMovement.x) > 0f)
      MoveHorizontally(ref deltaMovement);

    if (Math.Abs(deltaMovement.y) > 0f)
      MoveVertically(ref deltaMovement);

    transform.Translate(deltaMovement, Space.World);

    if (Time.deltaTime > 0)
      this.velocity = deltaMovement / Time.deltaTime;

    if (!this.collisionState.WasGroundedLastFrame && this.collisionState.Below)
      this.collisionState.BecameGroundedThisFrame = true;

    if (this.isGoingUpSlope)
      this.velocity.y = 0;

    if (ControllerCollided == null)
      return;

    foreach (var currentHit in this.raycastHitsThisFrame)
      ControllerCollided(currentHit);
  }

  public void WarpToGrounded()
  {
    do
    {
      Move(new Vector3(0, -1f, 0));
    } while (!IsGrounded);
  }

  public void RecalculateDistanceBetweenRays()
  {
    var colliderUseableHeight = BoxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2f * this.skinWidth);

    this.verticalDistanceBetweenRays = colliderUseableHeight / (this.totalHorizontalRays - 1);

    var colliderUseableWidth = BoxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2f * this.skinWidth);

    this.horizontalDistanceBetweenRays = colliderUseableWidth / (this.totalVerticalRays - 1);
  }

  private void PrimeRaycastOrigins()
  {
    var modifiedBounds = BoxCollider.bounds;

    modifiedBounds.Expand(-this.skinWidth);
    this.raycastOrigins.topLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
    this.raycastOrigins.bottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
    this.raycastOrigins.bottomLeft = modifiedBounds.min;
  }

  private void MoveHorizontally(ref Vector3 deltaMovement)
  {
    var isGoingRight = deltaMovement.x > 0;
    var rayDistance = Mathf.Abs(deltaMovement.x) + this.skinWidth;
    var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
    var initialRayOrigin = isGoingRight ? this.raycastOrigins.bottomRight : this.raycastOrigins.bottomLeft;

    for (var i = 0; i < this.totalHorizontalRays; i++)
    {
      var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * this.verticalDistanceBetweenRays);

      DrawRay(ray, rayDirection * rayDistance, Color.red);

      if (i == 0 && this.collisionState.WasGroundedLastFrame)
        this.raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, PlatformMask);
      else
        this.raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, PlatformMask & ~this.oneWayPlatformMask);

      if (!this.raycastHit)
        continue;

      if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(this.raycastHit.normal, Vector2.up)))
      {
        this.raycastHitsThisFrame.Add(this.raycastHit);
        break;
      }

      deltaMovement.x = this.raycastHit.point.x - ray.x;
      rayDistance = Mathf.Abs(deltaMovement.x);

      if (isGoingRight)
      {
        deltaMovement.x -= this.skinWidth;
        this.collisionState.Right = true;
      }
      else
      {
        deltaMovement.x += this.skinWidth;
        this.collisionState.Left = true;
      }

      this.raycastHitsThisFrame.Add(this.raycastHit);

      if (rayDistance < this.skinWidth + 0.001f)
        break;
    }
  }

  private bool HandleHorizontalSlope(ref Vector3 deltaMovement, float angle)
  {
    if (Mathf.RoundToInt(angle) == 90)
      return false;

    if (angle < this.slopeLimit)
    {
      if (!(deltaMovement.y < this.jumpingThreshold))
        return true;

      var slopeModifier = this.slopeSpeedMultiplier.Evaluate(angle);

      deltaMovement.x *= slopeModifier;
      deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
      this.isGoingUpSlope = true;

      this.collisionState.Below = true;
    }
    else
      deltaMovement.x = 0;

    return true;
  }

  private void MoveVertically(ref Vector3 deltaMovement)
  {
    var isGoingUp = deltaMovement.y > 0;
    var rayDistance = Mathf.Abs(deltaMovement.y) + this.skinWidth;
    var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
    var initialRayOrigin = isGoingUp ? this.raycastOrigins.topLeft : this.raycastOrigins.bottomLeft;
    var mask = PlatformMask;

    initialRayOrigin.x += deltaMovement.x;

    if (isGoingUp && !this.collisionState.WasGroundedLastFrame)
      mask &= ~this.oneWayPlatformMask;

    for (var i = 0; i < this.totalVerticalRays; i++)
    {
      var ray = new Vector2(initialRayOrigin.x + i * this.horizontalDistanceBetweenRays, initialRayOrigin.y);

      DrawRay(ray, rayDirection * rayDistance, Color.red);
      this.raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);

      if (!this.raycastHit)
        continue;

      deltaMovement.y = this.raycastHit.point.y - ray.y;
      rayDistance = Mathf.Abs(deltaMovement.y);

      if (isGoingUp)
      {
        deltaMovement.y -= this.skinWidth;
        this.collisionState.Above = true;
      }
      else
      {
        deltaMovement.y += this.skinWidth;
        this.collisionState.Below = true;
      }

      this.raycastHitsThisFrame.Add(this.raycastHit);

      if (!isGoingUp && deltaMovement.y > 0.00001f)
        this.isGoingUpSlope = true;

      if (rayDistance < this.skinWidth + 0.001f)
        return;
    }
  }

  private void HandleVerticalSlope(ref Vector3 deltaMovement)
  {
    var centerOfCollider = (this.raycastOrigins.bottomLeft.x + this.raycastOrigins.bottomRight.x) * 0.5f;
    var rayDirection = -Vector2.up;
    var slopeCheckRayDistance = SlopeLimitTangent * (this.raycastOrigins.bottomRight.x - centerOfCollider);
    var slopeRay = new Vector2(centerOfCollider, this.raycastOrigins.bottomLeft.y);

    DrawRay(slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow);
    this.raycastHit = Physics2D.Raycast(slopeRay, rayDirection, slopeCheckRayDistance, PlatformMask);

    if (!this.raycastHit)
      return;

    var angle = Vector2.Angle(this.raycastHit.normal, Vector2.up);

    if (Math.Abs(angle) < 0f)
      return;

    var isMovingDownSlope = this.raycastHit.normal.x.Sign() == deltaMovement.x.Sign();

    if (!isMovingDownSlope)
      return;

    var slopeModifier = this.slopeSpeedMultiplier.Evaluate(-angle);

    deltaMovement.y = this.raycastHit.point.y - slopeRay.y - SkinWidth;
    deltaMovement.x *= slopeModifier;
    this.collisionState.MovingDownSlope = true;
    this.collisionState.SlopeAngle = angle;
  }
}
