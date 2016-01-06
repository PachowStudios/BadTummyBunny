using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Level Installer")]
  public class LevelInstaller : MonoInstaller
  {
    [SerializeField] private CameraController cameraController = null;

    public override void InstallBindings()
    {
      Container.BindInstance(this.cameraController);
    }
  }
}