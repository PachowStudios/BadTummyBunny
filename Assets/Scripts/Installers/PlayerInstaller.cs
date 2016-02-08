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
    {
      Container
        .BindPublicFacade<Player>(InstallFacade)
        .WithExternal<IEventAggregator, PlayerView>();
    }

    private void InstallFacade(DiContainer subContainer)
    {
      subContainer.BindInstanceWithInterfaces(this.playerInstance);

      subContainer.BindInstance(this.playerSettings.Movement);
      subContainer.BindInstance(this.playerSettings.Health);
      subContainer.BindInstance(this.playerSettings.FartAimLean);

      subContainer.BindSingle<PlayerInput>();
      subContainer.BindSingleWithInterfaces<PlayerMovement>();
      subContainer.BindSingleWithInterfaces<PlayerHealth>();
      subContainer.BindSingleWithInterfaces<FartAimLean>();

      subContainer.Bind<IEventAggregator>().ToSingle<EventAggregator>();
    }
  }
}