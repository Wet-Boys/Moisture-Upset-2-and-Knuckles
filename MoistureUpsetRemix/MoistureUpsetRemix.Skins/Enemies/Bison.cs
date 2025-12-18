using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Skins.Utils;

namespace MoistureUpsetRemix.Skins.Enemies;

public class Bison : BaseEnemyReplacement
{
    public override SkinDefMoistureUpsetRemix SkinDef => AssetManager.LoadSkin("Thomas")!;
    public override string ConfigName => "Thomas";
    public override string ConfigDescription => "Replace Bison with Thomas.";
}