using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/World Map Installer")]
  public class WorldMapInstaller : MonoInstaller
  {
    [SerializeField] private CameraController cameraController = null;
    [SerializeField] private WorldMap worldMapInstance = null;
    [SerializeField] private WorldMapPlayer playerInstance = null;

    public override void InstallBindings()
    {
      InstallSceneBindings();
      InstallWorldMapBindings();
    }

    private void InstallSceneBindings()
    {
      Container.Bind<CameraController>().ToInstance(this.cameraController);
    }

    private void InstallWorldMapBindings()
    {
      Container.Bind<WorldMap>().ToInstance(this.worldMapInstance);
      Container.Bind<WorldMapPlayer>().ToInstance(this.playerInstance);
    }
  }
}