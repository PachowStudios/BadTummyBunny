using System.Linq;
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
    [SerializeField] private GameMenu gameMenuInstance = null;

    [Inject] private IReadOnlyDictionary<Scene, LevelSettings> LevelSettings { get; set; }
    [Inject] private IFactory<BaseStarSettings, IStar> StarFactory { get; set; }

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
      Container.BindInstanceWithInterfaces(this.gameMenuInstance);
    }

    private void InstallLevelSettings()
    {
      var settings = LevelSettings[this.scene];

      Container.BindBaseInstance(settings);

      if (!settings.Stars.IsNullOrEmpty())
        foreach (var star in settings.Stars.Select(StarFactory.Create))
          Container.BindInstanceWithInterfaces(star.GetType(), star);
    }

    private void InstallLevelHandlers()
    {
      Container.Bind<ILevelCompletionHandler>().ToSingle<LevelCompletionHandler>();
      Container.Bind<IInitializable>().ToSingle<StarCompletionHandler>();
    }
  }
}