using System.Collections.Generic;
using System.Linq;
using PachowStudios.BadTummyBunny.UserData;
using PachowStudios.Collections;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Global/Level Config Installer")]
  public class GlobalLevelSettingsInstaller : MonoInstaller
  {
    [SerializeField] private List<LevelSettings> levelSettings = null;

    private IReadOnlyDictionary<Scene, LevelSettings> MappedSettings { get; set; }

    public override void InstallBindings()
    {
      MappedSettings = this.levelSettings.ToDictionary(s => s.Scene).AsReadOnly();

      Container.BindInstance(MappedSettings);
      Container.BindIFactory<BaseStarSettings, StarProgress, IStar>().ToCustomFactory<StarFactory>();
    }
  }
}