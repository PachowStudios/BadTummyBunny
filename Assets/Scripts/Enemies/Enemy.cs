﻿using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	public float gravity = -35f;
	public float moveSpeed = 5f;
	public float groundDamping = 10f;
	public float airDamping = 5f;

	public float maxHealth = 25f;
	public int damage = 1;
	public Vector2 knockback = new Vector2(2f, 1f);
	public bool immuneToKnockback = false;

	public Color flashColor = new Color(1f, 0.47f, 0.47f, 1f);
	public float flashLength = 0.25f;

	public LayerMask blockVisibilityLayers;

	[SerializeField]
	protected Transform frontCheck;
	[SerializeField]
	protected Transform ledgeCheck;

	protected float health;

	protected Vector3 velocity;
	protected Vector3 lastGroundedPosition;
	protected float horizontalMovement = 0f;

	protected CharacterController2D controller;
	protected Animator animator;
	protected SpriteRenderer spriteRenderer;

	public float Health
	{
		get { return health; }
		set
		{
			health = Mathf.Clamp(value, 0f, maxHealth);
			CheckDeath();
		}
	}

	public bool IsGrounded => controller.isGrounded;

	protected bool Right => horizontalMovement > 0f;

	protected bool Left => horizontalMovement < 0f;

	protected bool FacingRight => transform.localScale.x > 0f;

	protected LayerMask CollisionLayers => controller.platformMask;

	protected bool PlayerIsOnRight => PlayerControl.Instance.transform.position.x > transform.position.x;

	protected float RelativePlayerLastGrounded => (lastGroundedPosition.y - PlayerControl.Instance.LastGroundedPosition.y).RoundToTenth();

	protected float RelativePlayerHeight => transform.position.y - PlayerControl.Instance.transform.position.y;

	protected virtual void Awake()
	{
		controller = GetComponent<CharacterController2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		health = maxHealth;
	}

	protected virtual void Update()
	{
		CalculateAI();
		ApplyAnimation();
	}

	protected virtual void LateUpdate()
	{
		GetMovement();
		ApplyMovement();
	}

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == Tags.Killzone)
			Kill();
	}

	protected virtual void OnTriggerStay2D(Collider2D other) => OnTriggerEnter2D(other);

	protected abstract void CalculateAI();

	protected abstract void ApplyAnimation();

	protected void GetMovement()
	{
		if (Right && !FacingRight)
			transform.Flip();
		else if (Left && FacingRight)
			transform.Flip();
	}

	protected void ApplyMovement()
	{
		float smoothedMovement = IsGrounded ? groundDamping : airDamping;

		velocity.x = Mathf.Lerp(velocity.x,
								horizontalMovement * moveSpeed,
								smoothedMovement * Time.deltaTime);
		velocity.y += gravity * Time.deltaTime;
		controller.move(velocity * Time.deltaTime);
		velocity = controller.velocity;

		if (IsGrounded)
		{
			velocity.y = 0f;
			lastGroundedPosition = transform.position;
		}
	}

	protected virtual void Jump(float height)
	{
		if (height <= 0f) return;

		velocity.y = Mathf.Sqrt(2f * height * -gravity);
		animator.SetTrigger("Jump");
	}

	protected bool CheckAtWall(bool flip = false)
	{
		Collider2D collision = Physics2D.OverlapPoint(frontCheck.position, CollisionLayers);
		bool atWall = collision != null;

		if (atWall && flip)
		{
			horizontalMovement *= -1f;

			if (horizontalMovement == 0f)
				horizontalMovement = Extensions.RandomSign();
		}

		return atWall;
	}

	protected bool CheckAtLedge(bool flip = false)
	{
		if (!IsGrounded) return false;

		Collider2D collision = Physics2D.OverlapPoint(ledgeCheck.position, CollisionLayers);
		bool atLedge = collision == null;

		if (atLedge && flip)
		{
			horizontalMovement *= -1f;

			if (horizontalMovement == 0f)
				horizontalMovement = Extensions.RandomSign();
		}

		return atLedge;
	}

	protected bool IsPlayerInRange(float min, float max)
	{
		int direction = FacingRight ? 1 : -1;
		Vector3 startPoint = new Vector3(transform.position.x + (min * direction), collider2D.bounds.center.y, 0f);
		Vector3 endPoint = startPoint + new Vector3((max - min) * direction, 0f, 0f);
		RaycastHit2D linecast = Physics2D.Linecast(startPoint, endPoint, LayerMask.GetMask("Player"));

		return linecast.collider != null;
	}

	protected bool IsPlayerVisible(float range = Mathf.Infinity)
	{
		RaycastHit2D linecast = Physics2D.Linecast(collider2D.bounds.center,
												   PlayerControl.Instance.collider2D.bounds.center,
												   blockVisibilityLayers);

		return linecast.collider == null && linecast.distance <= range;
	}

	protected void CheckDeath()
	{
		if (Health <= 0f)
			Kill();
	}

	protected void ResetColor() => spriteRenderer.color = Color.white;

	public void Kill()
	{
		ExplodeEffect.Instance.Explode(transform, velocity, spriteRenderer.sprite);
		Destroy(gameObject);
	}

	public void TakeDamage(int damage, Vector2 knockback, Vector2 direction)
	{
		if (Health <= 0f) return;

		direction.Scale(new Vector2(-1f, 1f));

		if (damage != 0f)
		{
			Health -= damage;

			if (Health > 0f)
			{
				ApplyKnockback(knockback, direction);
				spriteRenderer.color = flashColor;
				Wait.ForSeconds(flashLength, ResetColor);
			}
		}
	}

	public void ApplyKnockback(Vector2 knockback, Vector2 direction)
	{
		if (immuneToKnockback) return;

		knockback.x += Mathf.Sqrt(Mathf.Abs(Mathf.Pow(knockback.x, 2) * -gravity));

		if (IsGrounded)
			knockback.y += Mathf.Sqrt(Mathf.Abs(knockback.y * -gravity));

		knockback.Scale(direction);

		if (knockback.x != 0f || knockback.y != 0f)
		{
			velocity += knockback.ToVector3();
			controller.move(velocity * Time.deltaTime);
			velocity = controller.velocity;
		}
	}
}
