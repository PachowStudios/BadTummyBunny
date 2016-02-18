using System.Collections.Generic;
using System.Linq;
using PachowStudios.BadTummyBunny.UserData;
using PachowStudios.Collections;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class StarFactory : IFactory<Scene, IEnumerable<IStar>>
  {
    [Inject] private ISaveContainer SaveContainer { get; set; }
    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }

    public IEnumerable<IStar> Create(Scene scene)
      => Create(LevelSettings[scene], SaveContainer.SaveFile.GetLevel(scene)).ToList();

    private IEnumerable<IStar> Create(LevelSettings levelSettings, LevelProgress levelProgress)
      => levelSettings.Stars
        .Select(star => new Star(star, levelProgress.GetStar(star.Id)))
        .Cast<IStar>();
  }
}