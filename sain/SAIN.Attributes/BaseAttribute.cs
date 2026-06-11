using System;

namespace SAIN.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class BaseAttribute : Attribute
{
}
