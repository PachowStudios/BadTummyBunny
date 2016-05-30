using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny.UI
{
  [HideInPresenter]
  public class SceneUserInterface : UserInterface
  {
    [Inject(BindingIds.CameraUI)] private Camera UICamera { get; set; }

    [PostInject]
    private void PostInject()
      => this.RenderCamera.Value = UICamera;
  }
}