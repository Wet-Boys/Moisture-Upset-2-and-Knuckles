using System;
using UnityEngine;

namespace MoistureUpsetRemix.Common.Materials;

[Serializable]
public class MaterialPropertyOverride(string propertyName)
{
    public string propertyName = propertyName;
    
    public virtual void SetOnMaterial(Material target) {}
    
    public virtual void SetValue(object value) {}

    public virtual object GetValue() => throw new NotImplementedException();
}