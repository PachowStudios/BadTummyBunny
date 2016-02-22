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

    [SerializeField] private CameraController cameraControllerInstance = null;

    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }
    [Inject] private IFactory<Scene, IEnumerable<IStarController>>  StarControllerFactory { get; set; }

    public override void InstallBindings()
    {
      InstallInspectorSettings();
      InstallLevelSettings();
      InstallLevelHandlers();
    }

    private void InstallInspectorSettings()
    {
      Container.BindInstance(this.scene);
      Container.BindInstance(this.cameraControllerInstance);
    }

    private void InstallLevelSettings()
    {
      var settings = LevelSettings[this.scene];

      Container.BindBaseInstance(settings);

      StarControllerFactory.Create(this.scene).ForEach(Container.BindInstanceWithInterfaces);
    }

    private void InstallLevelHandlers()
    {
      Container.Bind<ILevelCompletionHandler>().ToSingle<LevelCompletionHandler>();
      Container.Bind<IInitializable>().ToSingle<StarCompletionHandler>();
    }
  }
}