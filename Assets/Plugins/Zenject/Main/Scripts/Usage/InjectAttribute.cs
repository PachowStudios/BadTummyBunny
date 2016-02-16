using System;
using JetBrains.Annotations;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class InjectAttribute : InjectAttributeBase
    {
        public InjectAttribute(string identifier)
        {
            Identifier = identifier;
        }

        public InjectAttribute()
        {
        }
    }
}

