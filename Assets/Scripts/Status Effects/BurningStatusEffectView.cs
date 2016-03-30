using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Status Effects/Burning Status Effect View")]
  public class BurningStatusEffectView : BaseView, IStatusEffectView
  {
    private ParticleSystem particleSystemComponent;

    private ParticleSystem ParticleSystem => this.GetComponentInChildrenIfNull(ref this.particleSystemComponent);

    public void Attach(IStatusEffectable affectedCharacter)
    {
      Transform.position = affectedCharacter.View.CenterPoint;
      Transform.parent = affectedCharacter.View.Transform;

      ParticleSystem.Play();
    }

    public void Detach()
    {
      ParticleSystem.DetachAndDestroy();
      Dispose();
    }
  }
}