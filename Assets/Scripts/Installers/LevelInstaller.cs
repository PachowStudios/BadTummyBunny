using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Level Installer")]
  public class LevelInstaller : MonoInstaller
  {
    [SerializeField] private CameraController cameraController = null;
    [SerializeField] private Player playerInstance = null;

    public override void InstallBindings()
    {
      // Bind scene objects
      Container.Bind<CameraController>().ToInstance(this.cameraController);
      
      // Bind player components
      Container.Bind<Player>().ToInstance(this.playerInstance);
      Container.Bind<IMovable>(Tags.Player).ToGetter<Player>(p => p.Movement);
      Container.Bind<IHasHealth>(Tags.Player).ToGetter<Player>(p => p.Health);
      Container.Bind<IScoreKeeper>(Tags.Player).ToGetter<Player>(p => p.Score);
      Container.Bind<IFartInfoProvider>(Tags.Player).ToGetter<Player>(p => p.FartInfo);
      Container.Bind<IHasHealthContainers>(Tags.Player).ToGetter<Player>(p => p.HealthContainers);
    }
  }
}