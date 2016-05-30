using System;
using JetBrains.Annotations;

namespace PachowStudios
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
  [MeansImplicitUse]
  public class DataBoundAttribute : Attribute { }
}