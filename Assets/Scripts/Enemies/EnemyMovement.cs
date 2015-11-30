using UnityEngine;

namespace BadTummyBunny
{
  public abstract class EnemyMovement : BaseMovable, IActivatable
  {
    [Header("Movement")]
    [SerializeField] private float groundDamping = 10f;
    [SerializeField] private float airDamping = 5f;
    [SerializeField] private bool deactivatedOutsideCamera = true;

    private ICharacter thisEnemy;
    private Animator animator;

    private bool isActivated;

    public bool IsActivated => this.isActivated || !this.deactivatedOutsideCamera;
    public int HorizontalMovement { get; set; }

    protected ICharacter ThisEnemy => this.GetInterfaceIfNull(ref this.thisEnemy);
    protected Animator Animator => this.GetComponentIfNull(ref this.animator);

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

      Velocity = Velocity.SetX(
        Mathf.Lerp(
          Velocity.x,
          HorizontalMovement * (MoveSpeedOverride ?? MoveSpeed),
          smoothedMovement * Time.deltaTime));

      Velocity = Velocity.AddY(Gravity * Time.deltaTime);
      Controller.Move(Velocity * Time.deltaTime);
      Velocity = Controller.Velocity;

      if (IsGrounded)
      {
        Velocity = Velocity.SetY(0f);
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
}
