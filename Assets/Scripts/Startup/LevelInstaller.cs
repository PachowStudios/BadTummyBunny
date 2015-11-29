using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Startup/Level Installer")]
  public class LevelInstaller : MonoInstaller
  {
    [SerializeField] private Player playerInstance = null;
    [SerializeField] private CameraController cameraController = null;

    public override void InstallBindings()
    {
      // Bind player components
      Container.Bind<Player>().ToInstance(this.playerInstance);
      Container.Bind<IMovable>(Tags.Player).ToGetter<Player>(p => p.Movement);
      Container.Bind<IHasHealth>(Tags.Player).ToGetter<Player>(p => p.Health);
      Container.Bind<IScoreKeeper>(Tags.Player).ToGetter<Player>(p => p.Score);
      Container.Bind<IFartInfoProvider>(Tags.Player).ToGetter<Player>(p => p.FartStatusProvider);
      Container.Bind<IHasHealthContainers>(Tags.Player).ToGetter<Player>(p => p.HealthContainers);

      // Bind scene objects
      Container.Bind<CameraController>().ToInstance(this.cameraController);
    }
  }
}