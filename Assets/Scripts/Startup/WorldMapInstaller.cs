using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Startup/World Map Installer")]
  public class WorldMapInstaller : MonoInstaller
  {
    [SerializeField] private WorldMap worldMapInstance = null;
    [SerializeField] private WorldMapPlayer playerInstance = null;

    public override void InstallBindings()
    {
      // Existing game objects
      Container.BindInstance(this.worldMapInstance);
      Container.BindInstance(this.playerInstance);
    }
  }
}