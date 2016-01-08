using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Farts/Fart View")]
  public class FartView : BaseView, IAttachable<PlayerView>
  {
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    [SerializeField] private PolygonCollider2D fartCollider = null;

    [InjectLocal] private IEventAggregator EventAggregator { get; set; }

    public List<ParticleSystem> Particles => this.particles;
    public PolygonCollider2D FartCollider => this.fartCollider;

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