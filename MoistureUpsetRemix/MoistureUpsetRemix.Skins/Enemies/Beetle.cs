using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Skins.Utils;

namespace MoistureUpsetRemix.Skins.Enemies;

public class Beetle : BaseEnemyReplacement
{
    public override SkinDefMoistureUpsetRemix SkinDef => AssetManager.LoadSkin("FroggyChair")!;
    public override string ConfigName => "Froggy Chair";
    public override string ConfigDescription => "Replace Beetle with Froggy Chair.";
}