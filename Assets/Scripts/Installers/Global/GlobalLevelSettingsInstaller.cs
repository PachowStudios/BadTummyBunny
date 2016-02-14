using System.Collections.Generic;
using System.Linq;
using PachowStudios.Collections;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Global/Level Config Installer")]
  public class GlobalLevelSettingsInstaller : MonoInstaller
  {
    [SerializeField] private List<LevelSettings> levelSettings = null;

    public override void InstallBindings()
    {
      Container.Bind<IReadOnlyDictionary<Scene, LevelSettings>>().ToInstance(
        this.levelSettings.ToDictionary(s => s.Scene).AsReadOnly());

      Container.BindIFactory<BaseStarSettings, IStar>().ToCustomFactory<StarFactory>();
    }
  }
}