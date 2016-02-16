using System.Collections.Generic;
using System.Linq;
using PachowStudios.Collections;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Fart Installer")]
  public class FartInstaller : MonoInstaller
  {
    [SerializeField] private List<FartSettings> fartSettings = null;

    private IReadOnlyDictionary<FartType, FartSettings> MappedSettings { get; set; }
    
    public override void InstallBindings()
    {
      MappedSettings = this.fartSettings.ToDictionary(s => s.Type).AsReadOnly();

      Container.BindInstance(MappedSettings);
      Container.BindIFactory<FartType, IFart>().ToCustomFactory<FartFactory>();
    }
  }
}