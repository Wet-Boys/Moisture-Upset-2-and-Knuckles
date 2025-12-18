using System;
using UnityEngine;

namespace MoistureUpsetRemix.Common.Materials;

[Serializable]
internal class VectorMaterialPropertyOverride(string propertyName, Vector4 value) : MaterialPropertyOverride(propertyName)
{
    public Vector4 value = value;

    public override void SetOnMaterial(Material target) => target.SetVector(propertyName, value);

    public override void SetValue(object value) => this.value = (Vector4)value;
    
    public override object GetValue() => value;
}