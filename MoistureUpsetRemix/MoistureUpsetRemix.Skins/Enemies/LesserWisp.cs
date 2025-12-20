using MoistureUpsetRemix.Common.AssetManagement;
using MoistureUpsetRemix.Skins.Utils;

namespace MoistureUpsetRemix.Skins.Enemies;

public class LesserWisp : BaseEnemyReplacement
{
    public override SkinDefMoistureUpsetRemix SkinDef => AssetManager.LoadSkin("DogPlane")!;
    public override string ConfigName => "Dog Plane";
    public override string ConfigDescription => "Replace Lesser Wisp with Dog Plane.";
}