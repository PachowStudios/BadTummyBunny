using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player/Player View")]
  public class PlayerView : CharacterView<Player>
  {
    [SerializeField] private Transform body = null;
    [SerializeField] private Transform fartPoint = null;

    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject(BindingIds.Global)] private IEventAggregator EventAggregator { get; set; }

    [Inject] public override Player Model { get; protected set; }

    public override Vector2 FacingDirection => new Vector2(Body.localScale.y, 0f);

    public Transform Body => this.body;
    public Transform FartPoint => this.fartPoint;

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
      => Model.Movement.PlayWalkingSound(rightStep == 1);

    protected override void LateUpdate()
    {
      base.LateUpdate();

      if (Model.FartInfo.IsFarting)
        CorrectRotationAndScale();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      switch (other.tag)
      {
        case Tags.Enemy:
          EventAggregator.Publish(new PlayerEnemyCollidedMessage(other.GetViewModel<IEnemy>()));
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

    private void CorrectRotationAndScale()
    {
      var rotation = Model.Movement.Velocity.DirectionToRotation2D();

      Body.localRotation = rotation;
      Body.localScale = Body.localScale
        .Set(y: Body.IsFlippedOnZAxis() ? -1f : 1f);
    }
  }
}