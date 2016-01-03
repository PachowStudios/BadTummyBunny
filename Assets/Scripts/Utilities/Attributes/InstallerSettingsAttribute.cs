using System;
using JetBrains.Annotations;

namespace PachowStudios
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  [MeansImplicitUse(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
  public class InstallerSettingsAttribute : Attribute { }
}