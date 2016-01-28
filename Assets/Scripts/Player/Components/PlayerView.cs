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
          LocalEventAggregator.Publish(new PlayerEnemyTriggeredMessage(other.GetViewModel<IEnemy>()));
          break;
        case Tags.Coin:
          LocalEventAggregator.Publish(new PlayerCoinTriggeredMessage(other.GetComponent<Coin>()));
          break;
        case Tags.Carrot:
          LocalEventAggregator.Publish(new PlayerCarrotTriggeredMessage(other.GetComponent<Carrot>()));
          break;
        case Tags.Flagpole:
          LocalEventAggregator.Publish(new PlayerFlagpoleTriggeredMessage(other.GetComponent<Flagpole>()));
          break;
        case Tags.RespawnPoint:
          LocalEventAggregator.Publish(new PlayerRespawnPointTriggeredMessage(other.GetComponent<RespawnPoint>()));
          break;
        case Tags.Killzone:
          LocalEventAggregator.Publish(new CharacterKillzoneTriggeredMessage());
          break;
      }
    }

    private void OnTriggerStay2D(Collider2D other)
      => OnTriggerEnter2D(other);

    [AnimationEvent]
    public void PlayWalkingSound(int rightStep)
      => Model.Movement.PlayWalkingSound(rightStep == 1);
  }
}