using System;
using PachowStudios.BadTummyBunny.UserData;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StarFactory : IFactory<BaseStarSettings, StarProgress, IStar>
  {
    [Inject] private IInstantiator Instantiator { get; set; }

    public IStar Create(BaseStarSettings settings, StarProgress progress)
      => (IStar)Instantiator.Instantiate(
        settings.Requirement.GetTypeMapping(),
        settings, progress);
  }
}