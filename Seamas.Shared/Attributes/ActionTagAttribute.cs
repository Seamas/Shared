using System;

namespace Wang.Seamas.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ActionTagAttribute(string name): Attribute
{
    public string Name { get; } = name;
}