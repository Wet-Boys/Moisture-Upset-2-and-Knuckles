using System;
using UnityEngine;

namespace MoistureUpsetRemix.Common.Materials;

[Serializable]
internal class TextureMaterialPropertyOverride(string propertyName, Texture value) : MaterialPropertyOverride(propertyName)
{
    public Texture value = value;
    
    public override void SetOnMaterial(Material target) => target.SetTexture(propertyName, value);

    public override void SetValue(object value) => this.value = (Texture)value;
    
    public override object GetValue() => value;
}