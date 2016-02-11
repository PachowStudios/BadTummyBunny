using System.ComponentModel;
using JetBrains.Annotations;

namespace PachowStudios.BadTummyBunny
{
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public enum Scene
  {
    [Description("World Map")]
    WorldMap,
    [Description("Level 1")]
    Level1
  }
}