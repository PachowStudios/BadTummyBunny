using System;
using System.Collections.Generic;
using System.Linq;
using PachowStudios.BadTummyBunny.UserData;
using PachowStudios.Collections;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StarControllerFactory : IFactory<Scene, IEnumerable<IStarController>>
  {
    [Inject] private IInstantiator Instantiator { get; set; }
    [Inject] private ISaveContainer SaveContainer { get; set; }
    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }

    public IEnumerable<IStarController> Create(Scene scene)
      => Create(LevelSettings[scene], SaveContainer.SaveFile.GetLevel(scene)).ToList();

    private IEnumerable<IStarController> Create(LevelSettings levelSettings, LevelProgress levelProgress)
      => from starSettings in levelSettings.Stars
         let starProgress = levelProgress.GetStar(starSettings.Id)
         where !starProgress.IsCompleted
         let star = new Star(starSettings, starProgress)
         select (IStarController)Instantiator.Instantiate(
           starSettings.Requirement.GetTypeMapping(),
           star, starSettings);
  }
}