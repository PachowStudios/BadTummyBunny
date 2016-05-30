using System.Collections.Generic;
using System.Linq.Extensions;
using PachowStudios.Collections;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Level Installer")]
  public class LevelInstaller : MonoInstaller
  {
    [SerializeField] private Scene scene = Scene.Level1;

    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }
    [Inject] private IFactory<Scene, IEnumerable<IStarController>>  StarControllerFactory { get; set; }

    public override void InstallBindings()
    {
      InstallLevelSettings();
      InstallLevelHandlers();
    }

    private void InstallLevelSettings()
    {
      Container.BindInstance(this.scene);
      Container.BindBaseInstance(LevelSettings[this.scene]);
      StarControllerFactory.Create(this.scene).ForEach(Container.BindInstanceWithInterfaces);
    }

    private void InstallLevelHandlers()
    {
      Container.Bind<ILevelCompletionHandler>().ToSingle<LevelCompletionHandler>();
      Container.Bind<IInitializable>().ToSingle<StarCompletionHandler>();
    }
  }
}