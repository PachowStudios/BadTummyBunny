using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Startup/World Map Installer")]
  public class WorldMapInstaller : MonoInstaller
  {
    [SerializeField] private CameraController cameraController = null;
    [SerializeField] private WorldMap worldMapInstance = null;
    [SerializeField] private WorldMapPlayer playerInstance = null;

    public override void InstallBindings()
    {
      // Bind scene objects
      Container.Bind<CameraController>().ToInstance(this.cameraController);

      // Bind world map objects
      Container.Bind<WorldMap>().ToInstance(this.worldMapInstance);
      Container.Bind<WorldMapPlayer>().ToInstance(this.playerInstance);
    }
  }
}