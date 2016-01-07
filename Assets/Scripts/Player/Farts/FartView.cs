using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class FartView : BaseView<Fart>, IAttachable<PlayerView>
  {
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    [SerializeField] private PolygonCollider2D fartCollider = null;

    [Inject] public override Fart Model { get; protected set; }

    public List<ParticleSystem> Particles => this.particles;
    public PolygonCollider2D FartCollider => this.fartCollider;

    [PostInject]
    private void Initialize()
      => name = Model.Name;

    public void Attach(PlayerView playerView)
    {
      Transform.position = playerView.FartPoint.position;
      Transform.rotation = playerView.FartPoint.rotation;
      Transform.parent = playerView.Body;
    }

    public void Detach()
      => Dispose();

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!Model.IsFarting)
        return;

      if (other.tag == Tags.Enemy)
        Model.OnEnemyTriggered(other.GetViewModel<IEnemy>());
    }
  }
}