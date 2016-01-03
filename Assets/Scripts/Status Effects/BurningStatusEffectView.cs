using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class BurningStatusEffectView : BaseView<BurningStatusEffect>, IStatusEffectView
  {
    private ParticleSystem particleSystemComponent;

    [Inject] public override BurningStatusEffect Model { get; protected set; }

    private ParticleSystem ParticleSystem => this.GetComponentIfNull(ref this.particleSystemComponent);

    public void Attach(IStatusEffectable affectedCharacter)
    {
      name = Model.Name;
      Transform.position = affectedCharacter.Movement.CenterPoint;
      Transform.parent = affectedCharacter.View.Transform;

      ParticleSystem.Play();
    }

    public void Detach()
      => ParticleSystem.DetachAndDestroy();
  }
}