using System;

namespace Wang.Seamas.Shared.Attributes;


[AttributeUsage(AttributeTargets.Class)]
public class ControllerTagAttribute(string name): Attribute
{
    public string Name { get; } = name;
}