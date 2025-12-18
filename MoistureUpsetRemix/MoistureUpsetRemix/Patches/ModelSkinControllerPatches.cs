using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MoistureUpsetRemix.Common.Logging;
using MoistureUpsetRemix.Enemies;
using RoR2;

namespace MoistureUpsetRemix.Patches;

public static class ModelSkinControllerPatches
{
    [HarmonyPatch]
    public static class ApplySkinAsyncPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(ModelSkinController), nameof(ModelSkinController.ApplySkinAsync)));
        }
        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            
            // Log.Debug(matcher.Instructions().ToDebugString());
            
            var skinReplacementMethodInfo = AccessTools.Method(typeof(ModelSkinControllerPatches), nameof(ApplySkinReplacementIfAvailable));

            matcher.MatchForward(true,
                new CodeMatch(code => code.opcode == OpCodes.Ldfld),
                new CodeMatch(code => code.opcode == OpCodes.Ldelem_Ref)
            );

            if (matcher.IsInvalid)
            {
                Log.Error("Failed to patch ModelSkinController! Skin replacements will not load!");
                return matcher.InstructionEnumeration();
            }

            matcher.Advance(1);
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1));
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Call, skinReplacementMethodInfo));
            
            // Log.Debug(matcher.Instructions().ToDebugString());
            
            return matcher.InstructionEnumeration();
        }
    }
    
    private static SkinDef ApplySkinReplacementIfAvailable(SkinDef inputSkinDef, ModelSkinController msc)
    {
        if (!EnemySkinReplacer.IsSkinEnabled(inputSkinDef.name))
            return inputSkinDef;
        
        var newSkin = EnemySkinReplacer.EnemySkinDef[inputSkinDef.name];

        // Somehow the skin didn't apply correctly, Seems to only happen in the LogBook
        if (newSkin.SkinIndex >= msc.skins.Length)
        {
            Log.Debug($"Somehow the skin {newSkin.name} didn't get added to the required prefab?");
            return newSkin.AsSkinDef()!;
        }

        return msc.skins[newSkin.SkinIndex];
    }
}