using System;
using JetBrains.Annotations;

namespace Zenject
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [MeansImplicitUse]
    public class InjectLocalAttribute : InjectAttributeBase
    {
        public InjectLocalAttribute(string identifier)
        {
            Identifier = identifier;
            LocalOnly = true;
        }

        public InjectLocalAttribute()
        {
            LocalOnly = true;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [MeansImplicitUse]
    public class InjectLocalOptionalAttribute : InjectAttributeBase
    {
        public InjectLocalOptionalAttribute(string identifier)
        {
            Identifier = identifier;
            IsOptional = true;
            LocalOnly = true;
        }

        public InjectLocalOptionalAttribute()
        {
            IsOptional = true;
            LocalOnly = true;
        }
    }
}

