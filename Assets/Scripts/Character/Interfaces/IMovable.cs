using UnityEngine;

public interface IMovable
{
	float Gravity { get; }
	float MoveSpeed { get; }
	Vector3 Position { get; }
	Vector3 LastGroundedPosition { get; }
	Vector3 Velocity { get; }
	Vector2 Direction { get; }
	bool IsGrounded { get; }
	bool WasGrounded { get; }
	LayerMask CollisionLayers { get; }

	void Move(Vector3 velocity);
	void ApplyKnockback(Vector2 knockback, Vector2 direction);
}