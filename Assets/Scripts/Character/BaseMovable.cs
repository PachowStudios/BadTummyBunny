using UnityEngine;

public abstract class BaseMovable : MonoBehaviour, IMovable
{
	[Header("Base Movement")]
	[SerializeField]
	protected float gravity = -35f;
	[SerializeField]
	protected float moveSpeed = 5f;

	[Header("Base Components")]
	[SerializeField]
	protected CharacterController2D controller;

	protected Vector3 velocity = default(Vector3);

	public virtual float Gravity => gravity;
	public virtual float MoveSpeed => moveSpeed;
	public virtual Vector3 Position => transform.position;
	public virtual Vector3 LastGroundedPosition { get; protected set; }
	public virtual Vector3 Velocity => velocity;
	public virtual Vector2 Direction => velocity.normalized;
	public virtual bool IsGrounded => controller.isGrounded;
	public virtual bool WasGrounded => controller.wasGroundedLastFrame;
	public virtual LayerMask CollisionLayers => controller.platformMask;

	public abstract void Move(Vector3 velocity);

	public virtual void ApplyKnockback(Vector2 knockback, Vector2 direction)
	{
		if (knockback.IsZero()) return;

		knockback.x += Mathf.Sqrt(Mathf.Abs(Mathf.Pow(knockback.x, 2) * -Gravity));

		if (IsGrounded)
			velocity.y = Mathf.Sqrt(Mathf.Abs(knockback.y * -Gravity));

		knockback.Scale(direction);

		if (knockback.IsZero())
		{
			velocity += knockback.ToVector3();
			controller.move(velocity * Time.deltaTime);
			velocity = controller.velocity;
		}
	}

	protected virtual void Jump(float height)
	{
		if (height <= 0f) return;

		velocity.y = Mathf.Sqrt(2f * height * -gravity);
	}
}