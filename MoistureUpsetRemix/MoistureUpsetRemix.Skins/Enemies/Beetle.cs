using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Skins.Utils;

namespace MoistureUpsetRemix.Skins.Enemies;

public class Beetle : BaseEnemyReplacement
{
    protected override SkinDefMoistureUpsetRemix SkinDef => AssetManager.LoadSkin("FroggyChair")!;
}