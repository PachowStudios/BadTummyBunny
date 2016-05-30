using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/World Map Installer")]
  public class WorldMapInstaller : MonoInstaller
  {
    [SerializeField] private WorldMap worldMap = null;
    [SerializeField] private WorldMapPlayer player = null;

    public override void InstallBindings()
    {
      Container.BindInstance(this.worldMap);
      Container.BindInstance(this.player);
    }
  }
}