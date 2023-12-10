using HarmonyLib;
using RimWorld;
using TR.TGraphics;
using TR.TGraphics.TextureContent;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace TR;

public static class MiscPatches
{
    //
    [HarmonyPatch(typeof(MemoryUtility))]
    [HarmonyPatch(nameof(MemoryUtility.ClearAllMapsAndWorld))]
    internal static class MemoryUtility_ClearAllMapsAndWorldPatch
    {
        public static void Postfix()
        {
            StaticData.Notify_ClearingMapAndWorld();
        }
    }

    //Patching the vanilla shader def to allow custom shaders
    [HarmonyPatch(typeof(ShaderTypeDef))]
    [HarmonyPatch("Shader", MethodType.Getter)]
    public static class ShaderPatch
    {
        public static bool Prefix(ShaderTypeDef __instance, ref Shader __result, ref Shader ___shaderInt)
        {
            if (__instance is TRShaderTypeDef)
            {
                if (___shaderInt == null)
                {
                    ___shaderInt = TRContentDatabase.LoadShader(__instance.shaderPath);
                }
                __result = ___shaderInt;
                return false;
            }
            return true;
        }
    }
}