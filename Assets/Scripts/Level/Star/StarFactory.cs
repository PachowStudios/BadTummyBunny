using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StarFactory : IFactory<BaseStarSettings, IStar>
  {
    [Inject] private IInstantiator Instantiator { get; set; }

    public IStar Create(BaseStarSettings settings)
      => (IStar)Instantiator.Instantiate(
        settings.Requirement.GetTypeMapping(),
        settings);
  }
}