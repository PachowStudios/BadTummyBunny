using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Farts/Fart View")]
  public class FartView : BaseView, IAttachable<PlayerView>
  {
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();

    private PolygonCollider2D fartColliderComponent = null;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    public List<ParticleSystem> Particles => this.particles;
    public PolygonCollider2D FartCollider => this.GetComponentIfNull(ref this.fartColliderComponent);

    public void Attach(PlayerView playerView)
    {
      Transform.AlignWith(playerView.FartPoint);
      Transform.parent = playerView.Body;
    }

    public void Detach()
      => Dispose();

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == Tags.Enemy)
        EventAggregator.Publish(new FartEnemyTriggeredMessage(other.GetViewModel<IEnemy>()));
    }
  }
}