namespace MoistureUpsetRemix.Enemies;

public class AudioReplacements
{
    public static void ReplaceAudio()
    {
        On.EntityStates.GenericCharacterSpawnState.OnEnter += (orig, self) =>
        {
            switch (self.characterBody.baseNameToken)
            {
                case "BEETLE_BODY_NAME":
                    self.spawnSoundString = "Play_FrogChair_Spawn";
                    self.sfxLocator.barkSound = "Play_FrogChair_Passive";
                    self.sfxLocator.deathSound = "Play_FrogChair_Death";
                    EntityStates.BeetleMonster.HeadbuttState.attackSoundString = "Play_FrogChair_Attack";
                    break;
                default:
                    DebugClass.Log(self.characterBody.baseNameToken);
                    break;
            }
            orig(self);
        };
    }
}