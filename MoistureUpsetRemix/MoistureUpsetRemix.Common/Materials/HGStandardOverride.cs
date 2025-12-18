using UnityEngine;

namespace MoistureUpsetRemix.Common.Materials;

[CreateAssetMenu(fileName = "HGStandardOverride", menuName = "MoistureUpsetRemix/HGStandardOverride", order = 2)]
public class HGStandardOverride : BaseMaterialOverride
{
    public override string MockShaderPath => "Assets/MoistureUpsetRemix/Shaders/HGStandardMock.shader";
}