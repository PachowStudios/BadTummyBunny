using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public interface IMovable : IActivatable
  {
    float Gravity { get; }
    float MoveSpeed { get; }
    Vector3 LastGroundedPosition { get; }
    Vector3 Velocity { get; }
    Vector2 MovementDirection { get; }
    bool IsFalling { get; }
    bool IsGrounded { get; }
    bool WasGrounded { get; }
    LayerMask CollisionLayers { get; }

    void Move(Vector3 moveVelocity);
    bool Jump(float jumpHeight);
    void ApplyKnockback(Vector2 knockback, Vector2 direction);
    void Disable();
  }
}
