using System;
using UnityEngine;

namespace MoistureUpsetRemix.Common.Materials;

[Serializable]
internal class ColorMaterialPropertyOverride(string propertyName, Color value) : MaterialPropertyOverride(propertyName)
{
    public Color value = value;

    public override void SetOnMaterial(Material target) => target.SetColor(propertyName, value);

    public override void SetValue(object value) => this.value = (Color)value;

    public override object GetValue() => value;
}