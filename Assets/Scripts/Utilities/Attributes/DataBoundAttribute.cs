using System;
using JetBrains.Annotations;

namespace PachowStudios
{
  [AttributeUsage(AttributeTargets.Field)]
  [MeansImplicitUse]
  public class DataBoundAttribute : Attribute { }
}