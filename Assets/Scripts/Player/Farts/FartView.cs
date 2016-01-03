using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class FartView : BaseView<BasicFart>
  {
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    [SerializeField] private PolygonCollider2D fartCollider = null;

    [Inject] public override BasicFart Model { get; protected set; }

    public List<ParticleSystem> Particles => this.particles;
    public PolygonCollider2D FartCollider => this.fartCollider; 

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!Model.IsFarting)
        return;

      if (other.tag == Tags.Enemy)
        Model.OnEnemyTriggered(other.GetViewModel<ICharacter>());
    }
  }
}