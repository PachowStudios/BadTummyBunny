using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Player Installer")]
  public class PlayerInstaller : MonoInstaller
  {
    [SerializeField] private PlayerView playerInstance = null;
    [SerializeField] private Player.Settings playerSettings = null;

    public override void InstallBindings()
    {
      Container.BindFacade<Player>(InstallFacade).ToSingle();
      Container.BindAllInterfacesToSingle<Player>();
    }

    private void InstallFacade(DiContainer subContainer)
    {
      subContainer.Bind<IEventAggregator>().ToSingle<EventAggregator>();

      subContainer.BindInstance(this.playerSettings.Movement);
      subContainer.BindInstance(this.playerSettings.Health);
      subContainer.BindInstanceWithInterfaces(this.playerInstance);

      subContainer.BindSingleWithInterfaces<PlayerMovement>();
      subContainer.BindSingleWithInterfaces<PlayerHealth>();
    }
  }
}