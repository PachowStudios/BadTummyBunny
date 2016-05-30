using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Camera Installer")]
  public class CameraInstaller : MonoInstaller
  {
    [SerializeField] private CameraController cameraController = null;
    [SerializeField] private Camera uiCamera = null;

    public override void InstallBindings()
    {
      Container.BindInstance(this.cameraController);
      Container.BindInstance(BindingIds.CameraMain, this.cameraController.Camera);
      Container.BindInstance(BindingIds.CameraUI, this.uiCamera);
    }
  }
}
