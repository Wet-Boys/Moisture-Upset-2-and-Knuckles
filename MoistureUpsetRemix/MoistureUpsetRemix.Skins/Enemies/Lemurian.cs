using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Skins.Utils;

namespace MoistureUpsetRemix.Skins.Enemies;

public class Lemurian : BaseEnemyReplacement
{
    protected override SkinDefMoistureUpsetRemix SkinDef => AssetManager.LoadSkin("Mike")!;
}