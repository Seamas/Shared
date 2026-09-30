using System;

namespace Wang.Seamas.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LoginRequiredAttribute : Attribute
{
}