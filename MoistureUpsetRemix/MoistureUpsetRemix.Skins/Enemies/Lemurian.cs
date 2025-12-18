using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Skins.Utils;

namespace MoistureUpsetRemix.Skins.Enemies;

public class Lemurian : BaseEnemyReplacement
{
    public override SkinDefMoistureUpsetRemix SkinDef => AssetManager.LoadSkin("Mike")!;
    public override string ConfigName => "Mike";
    public override string ConfigDescription => "Replace Lemurian with Mike.";
}