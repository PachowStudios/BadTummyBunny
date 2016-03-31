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

    public bool CanHitEnemies
    {
      get { return FartCollider.enabled; }
      set { FartCollider.enabled = value; }
    }

    public PolygonCollider2D FartCollider => this.GetComponentIfNull(ref this.fartColliderComponent);

    public Vector3 FartOrigin => FartCollider.transform.position;

    [Inject] private IEventAggregator EventAggregator { get; set; }

    public void Attach(PlayerView playerView)
    {
      Transform.AlignWith(playerView.FartPoint);
      Transform.parent = playerView.Body;
    }

    public void Detach()
      => Dispose();

    public void StartParticles()
      => this.particles.ForEach(p => p.Play());

    public void StopParticles()
      => this.particles.ForEach(p => p.Stop());

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (CanHitEnemies && other.tag == Tags.Enemy)
        EventAggregator.Publish(new FartEnemyTriggeredMessage(other.GetViewModel<IEnemy>()));
    }
  }
}