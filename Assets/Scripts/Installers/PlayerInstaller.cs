using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Player Installer")]
  public class PlayerInstaller : MonoInstaller
  {
    [SerializeField] private PlayerView playerInstance = null;
    [SerializeField] private PlayerSettings playerSettings = null;

    public override void InstallBindings()
      => Container.BindFacade<Player>(InstallFacade).ToSingle();

    private void InstallFacade(DiContainer subContainer)
    {
      subContainer.Bind<IEventAggregator>().ToSingle<EventAggregator>();

      subContainer.BindAbstractInstance(this.playerSettings.Movement);
      subContainer.BindInstance(this.playerSettings.Health);
      subContainer.BindInstance(this.playerSettings.FartAimLean);
      subContainer.BindInstanceWithInterfaces(this.playerInstance);

      subContainer.BindSingle<PlayerInput>();
      subContainer.BindSingleWithInterfaces<PlayerMovement>();
      subContainer.BindSingleWithInterfaces<PlayerHealth>();
      subContainer.BindSingleWithInterfaces<FartAimLean>();
    }
  }
}