using System;

namespace MoistureUpsetRemix.Common.AssetManagement;

[AttributeUsage(AttributeTargets.Class)]
public class RunMethodOnAssetsLoadedAttribute(string methodName) : Attribute
{
    public readonly string MethodName = methodName;
}