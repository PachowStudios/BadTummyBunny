using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IMovable : IActivatable
  {
    float Gravity { get; }
    float MoveSpeed { get; }
    Collider2D Collider { get; }
    Vector3 Position { get; }
    Vector3 CenterPoint { get; }
    Vector3 LastGroundedPosition { get; }
    Vector3 Velocity { get; }
    Vector2 MovementDirection { get; }
    Vector2 FacingDirection { get; }
    bool IsFacingRight { get; }
    bool IsFalling { get; }
    bool IsGrounded { get; }
    bool WasGrounded { get; }
    LayerMask CollisionLayers { get; }

    Vector3 Move(Vector3 moveVelocity);
    void SetPosition(Vector3 position);
    void Flip();
    bool Jump(float jumpHeight);
    void ApplyKnockback(Vector2 knockback, Vector2 direction);
    void Disable();
  }
}
