using System;

namespace PachowStudios
{
  [AttributeUsage(AttributeTargets.Field)]
  public class TypeMappingAttribute : Attribute
  {
    public Type Type { get; private set; }

    public TypeMappingAttribute(Type type)
    {
      Type = type;
    }
  }
}