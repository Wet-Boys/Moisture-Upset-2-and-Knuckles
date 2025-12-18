namespace MoistureUpsetRemix.Skins.Enemies;

public abstract class BaseEnemyReplacement
{
    public abstract SkinDefMoistureUpsetRemix SkinDef { get; }
    
    public abstract string ConfigName { get; }
    
    public abstract string ConfigDescription { get; }

    public void Apply()
    {
        var skinDef = SkinDef;
        BeforeApply(skinDef);
        
        skinDef.AddEnemySkinToPrefab();
        
        AfterApply(skinDef);
    }
    
    protected virtual void BeforeApply(SkinDefMoistureUpsetRemix skinDef)
    {
        
    }
    
    protected virtual void AfterApply(SkinDefMoistureUpsetRemix skinDef)
    {
        
    }
}