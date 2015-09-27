using UnityEngine;

public abstract class EnemyMovement : BaseMovable, IActivatable
{
  [Header("Movement")]
  [SerializeField] protected float groundDamping = 10f;
  [SerializeField] protected float airDamping = 5f;
  [SerializeField] private bool deactivatedOutsideCamera = true;

  [Header("Components")]
  [SerializeField] protected Animator animator = null;

  private ICharacter thisEnemy;
  private bool isActivated;

  public bool IsActivated => this.isActivated || !this.deactivatedOutsideCamera;
  public int HorizontalMovement { get; set; } = 0;

  protected ICharacter ThisEnemy => this.GetInterfaceIfNull(ref this.thisEnemy);
  protected bool IsMovingRight => HorizontalMovement > 0;
  protected bool IsMovingLeft => HorizontalMovement < 0;

  private void Update()
  {
    if (IsActivated)
      InternalUpdate();
  }

  protected virtual void InternalUpdate() { }

  protected virtual void LateUpdate()
  {
    if (!IsActivated)
      return;

    GetMovement();
    ApplyMovement();
  }

  protected virtual void GetMovement()
  {
    if ((IsMovingRight && !IsFacingRight) ||
        (IsMovingLeft && IsFacingRight))
      Flip();
  }

  protected virtual void ApplyMovement()
  {
    var smoothedMovement = IsGrounded ? this.groundDamping : this.airDamping;

    this.velocity.x = Mathf.Lerp(this.velocity.x,
                                 HorizontalMovement * (MoveSpeedOverride ?? MoveSpeed),
                                 smoothedMovement * Time.deltaTime);
    this.velocity.y += this.gravity * Time.deltaTime;
    this.controller.Move(this.velocity * Time.deltaTime);
    this.velocity = this.controller.Velocity;

    if (IsGrounded)
    {
      this.velocity.y = 0f;
      LastGroundedPosition = Position;
    }
  }

  public override bool Jump(float height)
  {
    if (!base.Jump(height))
      return false;

    this.animator.SetTrigger("Jump");

    return true;
  }

  public virtual void Activate()
    => this.isActivated = true;

  public virtual void Deactivate()
    => this.isActivated = false;
}
