using System;
using UnityEngine;

namespace MoistureUpsetRemix.Common.Materials;

[Serializable]
internal class FloatMaterialPropertyOverride(string propertyName, float value) : MaterialPropertyOverride(propertyName)
{
    public float value = value;
    
    public override void SetOnMaterial(Material target) => target.SetFloat(propertyName, value);

    public override void SetValue(object value) => this.value = (float)value;
    
    public override object GetValue() => value;
}