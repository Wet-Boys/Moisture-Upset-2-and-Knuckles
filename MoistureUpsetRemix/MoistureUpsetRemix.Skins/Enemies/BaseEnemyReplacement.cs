namespace MoistureUpsetRemix.Skins.Enemies;

public abstract class BaseEnemyReplacement
{
    protected abstract SkinDefMoistureUpsetRemix SkinDef { get; }

    public void Apply()
    {
        var skinDef = SkinDef;
        BeforeApply(skinDef);
        
        skinDef.ReplaceEnemySkin();
        
        AfterApply(skinDef);
    }
    
    protected virtual void BeforeApply(SkinDefMoistureUpsetRemix skinDef)
    {
        
    }
    
    protected virtual void AfterApply(SkinDefMoistureUpsetRemix skinDef)
    {
        
    }
}