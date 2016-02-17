using System;
using System.Collections.Generic;
using System.Linq;
using PachowStudios.BadTummyBunny.UserData;
using PachowStudios.Collections;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseStarFactory : IFactory<Scene, IEnumerable<IStar>>
  {
    [Inject] protected IInstantiator Instantiator { get; private set; }

    [Inject] private ISaveContainer SaveContainer { get; set; }
    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }

    public IEnumerable<IStar> Create(Scene scene)
      => Create(LevelSettings[scene], SaveContainer.SaveFile.GetLevel(scene)).ToList();

    protected abstract IEnumerable<IStar> Create(LevelSettings levelSettings, LevelProgress levelProgress);
  }

  public class StarInfoFactory : BaseStarFactory
  {
    protected override IEnumerable<IStar> Create(LevelSettings levelSettings, LevelProgress levelProgress)
      => levelSettings.Stars.Select(starSettings => (IStar)Instantiator.Instantiate(
        starSettings.Requirement.GetTypeMapping(),
        starSettings,
        levelProgress.GetStar(starSettings.Id)));
  }

  public class StarControllerFactory : BaseStarFactory
  {
    protected override IEnumerable<IStar> Create(LevelSettings levelSettings, LevelProgress levelProgress)
      => from starSettings in levelSettings.Stars
         let starProgress = levelProgress.GetStar(starSettings.Id)
           where !starProgress.IsCompleted
           select (IStar)Instantiator.Instantiate(
             starSettings.Requirement.GetTypeMapping(),
             starSettings,
             levelProgress.GetStar(starSettings.Id));
  }
}