using System;
using JetBrains.Annotations;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [MeansImplicitUse]
    public class PostInjectAttribute : Attribute
    {
    }
}

