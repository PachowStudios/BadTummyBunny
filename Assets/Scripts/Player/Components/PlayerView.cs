using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player/Player View")]
  public class PlayerView : FacadeView<Player>
  {
    [SerializeField] private Transform body = null;
    [SerializeField] private Transform fartPoint = null;

    [InjectLocal] private IEventAggregator LocalEventAggregator { get; set; }

    [Inject(BindingIds.Global)] private IEventAggregator EventAggregator { get; set; }

    [Inject] public override Player Model { get; protected set; }

    public Transform Body => this.body;
    public Transform FartPoint => this.fartPoint;

    protected override void LateUpdate()
    {
      base.LateUpdate();
      
      if (Model.FartInfo.IsFarting)
        Body.CorrectScaleForRotation(Model.Movement.Velocity.DirectionToRotation2D());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      LocalEventAggregator.Publish(new PlayerCollidedMessage(other));

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

    // This generates too much garbage every frame. We might not need it.
    //private void OnTriggerStay2D(Collider2D other)
    //  => OnTriggerEnter2D(other);

    [AnimationEvent]
    public void PlayWalkingSound(int rightStep)
      => Model.Movement.PlayWalkingSound(rightStep == 1);
  }
}