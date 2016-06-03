using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player/Player View")]
  public class PlayerView : CharacterView<Player>
  {
    [SerializeField] private Transform body = null;
    [SerializeField] private Transform fartPoint = null;

    private float flashTimer;
    private float smoothFlashTime;

    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject(BindingIds.Global)] private IEventAggregator EventAggregator { get; set; }

    [Inject] public override Player Model { get; protected set; }

    public override Vector2 FacingDirection => new Vector2(Body.localScale.y, 0f);

    public Transform Body => this.body;
    public Transform FartPoint => this.fartPoint;

    private PlayerHealth Health => Model.Health;
    private PlayerMovement Movement => Model.Movement;
    private IFartInfoProvider FartInfo => Model.FartInfo;

    private bool IsFacingMovementDirection
      => Movement.HorizontalMovement >= 0f && IsFacingRight
         || Movement.HorizontalMovement <= 0f && !IsFacingRight;

    public override void Flip()
      => Body.FlipViaRotation();

    public void ResetOrientation()
    {
      var isFacingRight = Body.IsFlippedOnZAxis();

      Body.localScale = Vector3.one;
      Body.localRotation = Quaternion.identity;

      if (isFacingRight)
        Flip();
    }

    [AnimationEvent]
    public void PlayWalkingSound(int rightStep)
      => Movement.PlayWalkingSound(rightStep == 1);

    protected override void Update()
    {
      base.Update();

      if (!Health.IsDead)
        UpdateInvincibilityFlash();
    }

    protected override void LateUpdate()
    {
      base.LateUpdate();

      if (FartInfo.IsFarting)
        CorrectRotationAndScale();
      else if (!IsFacingMovementDirection)
        Flip();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      switch (other.tag)
      {
        case Tags.Enemy:
          EventAggregator.Publish(new PlayerEnemyCollidedMessage(other.GetModel<IEnemy>()));
          break;
        case Tags.Coin:
          EventAggregator.Publish(new PlayerCoinCollectedMessage(other.GetComponent<Coin>()));
          break;
        case Tags.Carrot:
          EventAggregator.Publish(new PlayerCarrotCollectedMessage(other.GetComponent<Carrot>()));
          break;
        case Tags.Flagpole:
          EventAggregator.Publish(new PlayerFlagpoleActivatedMessage(other.GetComponent<Flagpole>()));
          break;
        case Tags.RespawnPoint:
          EventAggregator.Publish(new PlayerRespawnPointActivatedMessage(other.GetComponent<RespawnPoint>()));
          break;
        case Tags.Killzone:
          LocalEventAggregator.Publish(new CharacterKillzoneTriggeredMessage());
          break;
      }
    }

    private void UpdateInvincibilityFlash()
    {
      const float FlashTime = 0.25f;

      if (!Health.IsInvincible)
      {
        SetRenderersEnabled(true);
        this.smoothFlashTime = FlashTime;

        return;
      }

      this.flashTimer += Time.deltaTime;
      this.smoothFlashTime = this.smoothFlashTime.LerpTo(0.05f, 0.025f);

      if (this.flashTimer > this.smoothFlashTime)
      {
        AlternateRenderersEnabled();
        this.flashTimer = 0f;
      }
    }

    private void CorrectRotationAndScale()
    {
      var rotation = Movement.Velocity.DirectionToRotation2D();

      Body.localRotation = rotation;
      Body.localScale = Body.localScale
        .Set(y: Body.IsFlippedOnZAxis() ? -1f : 1f);
    }
  }
}