using System;
using JetBrains.Annotations;

namespace PachowStudios
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  [MeansImplicitUse(ImplicitUseTargetFlags.Members)]
  public class InstallerSettingsAttribute : Attribute { }
}