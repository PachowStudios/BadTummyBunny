using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Level Installer")]
  public class LevelInstaller : MonoInstaller
  {
    [SerializeField] private CameraController cameraControllerInstance = null;
    [SerializeField] private GameMenu gameMenuInstance = null;

    public override void InstallBindings()
    {
      Container.BindInstance(this.cameraControllerInstance);
      Container.BindInstanceWithInterfaces(this.gameMenuInstance);
    }
  }
}