﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TeleCore;
using TeleCore.Network;
using TeleCore.Network.Utility;
using UnityEngine;
using Verse;

namespace TR;

internal static class TRUIPatches
{
    //Tiberium World Coverage Option
    [HarmonyPatch(typeof(Page_CreateWorldParams))]
    [HarmonyPatch(nameof(Page_CreateWorldParams.DoWindowContents))]
    public static class WindowContentsPatch
    {
        static readonly MethodInfo changeMethod = AccessTools.Method(typeof(WindowContentsPatch), nameof(ChangeTiberiumCoverage));
        static readonly MethodInfo callOperand = AccessTools.Method(typeof(Widgets), nameof(Widgets.EndGroup));

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Call && code.Calls(callOperand))
                {
                    //"TR.World.CoverageSlider"
                    TRLog.Message($"Patching Opcodes! Next: {code.opcode} | {code.operand}");
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 7);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                    yield return new CodeInstruction(OpCodes.Call, changeMethod);
                }
                yield return code;
            }
        }

        public static void ChangeTiberiumCoverage(float curY, float width)
        {
            curY += 40;
            float fullWidth = 200 + width;
            var imageRect = new Rect(0, curY - 8f, fullWidth, 40);
            Widgets.DrawShadowAround(imageRect);
            GenUI.DrawTextureWithMaterial(imageRect, TRContent.TibOptionBG_Cut, null, new Rect(0, 0, 1, 1));
            Widgets.Label(new Rect(0f, curY, 200f, 30f), "TR.World.CoverageSlider".Translate());
            Rect newRect = new Rect(200, curY, width, 30f);
            TiberiumSettings.Settings.tiberiumCoverage = Widgets.HorizontalSlider_NewTemp(newRect, (float)TiberiumSettings.Settings.tiberiumCoverage, 0f, 1, true, "Medium", "None", "Full", 0.05f);
        }
    }

    //Readout
    [HarmonyPatch(typeof(ResourceReadout))]
    [HarmonyPatch(nameof(ResourceReadout.ResourceReadoutOnGUI))]
    public static class ResourceReadoutOnGUIPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo helper = AccessTools.Method(typeof(ResourceReadoutOnGUIPatch), nameof(AdjustResourceReadoutDownwards));

            bool patched = false;
            foreach (var code in instructions)
            {
                yield return code;
                if (code.opcode == OpCodes.Stloc_0 && !patched)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0); //Rect on stack  
                    yield return new CodeInstruction(OpCodes.Call, helper); //Consumes 1 and returns Rect
                    yield return new CodeInstruction(OpCodes.Stloc_0);
                    patched = true;
                }
            }

        }

        public static void Postfix(ResourceReadout __instance)
        {
            if (ShouldFix)
                DrawCredits();
        }

        static Rect AdjustResourceReadoutDownwards(Rect rect)
        {
            if (!ShouldFix) return rect;

            Rect newRect = new Rect(rect);
            newRect.yMin += TotalHeight.Value + 10f;
            return newRect;
        }

        private static bool ShouldFix => GetTiberiumCredits(Find.CurrentMap) > 0;

        static double GetTiberiumCredits(Map map)
        {
            return GetNetwork(map)?.System.TotalValue ?? 0;
        }

        private static float? TotalHeight = 120;
        private static float? ResourceReadoutHeight = 60f;

        static PipeNetwork GetNetwork(Map map)
        {
            var currentSet = map.GetMapInfo<PipeNetworkMapInfo>()[TiberiumDefOf.TiberiumNetwork]?.TotalPartSet;
            if(currentSet == null) return null;
            if(currentSet.FullSet.Count == 0) return null;
            return currentSet.FullSet.First()?.Network;
            //return map.MapInfo<NetworkMapInfo>()[TiberiumDefOf.TiberiumNetwork]?.MainNetworkPart?.Network;
        }

        static void DrawCredits()
        {
            //
            Rect mainRect = new Rect(5f, 10, 120f, TotalHeight.Value);
            string creditLabel = "TR_Credits".Translate();
            Vector2 creditLabelSize = Text.CalcSize(creditLabel);
            Rect creditLabelRect = new Rect(5, mainRect.y, mainRect.width, creditLabelSize.y + 8);
            Rect readoutRect = new Rect(5, creditLabelRect.yMax, mainRect.width, ResourceReadoutHeight.Value);

            //Main
            //Widgets.DrawWindowBackground(mainRect);
            TRWidgets.DrawColoredBox(mainRect, new Color(1, 1, 1, 0.125f), Color.white, 1);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(creditLabelRect, creditLabel);
            Text.Anchor = default;
            Widgets.DrawLine(new Vector2(5f, creditLabelRect.yMax), new Vector2(125f, creditLabelRect.yMax), Color.white, 1f);

            ResourceReadoutHeight = FlowUI<NetworkValueDef>.DrawFlowValueStackReadout(readoutRect, GetNetwork(Find.CurrentMap).System.TotalStack);

            Text.Font = GameFont.Tiny;
            string totalLabel = "TR_CreditsTotal".Translate(Math.Round(GetTiberiumCredits(Find.CurrentMap)));
            Vector2 totalLabelSize = Text.CalcSize(totalLabel);
            Rect totalLabelRect = new Rect(10f, readoutRect.yMax, mainRect.width, totalLabelSize.y);
            Widgets.Label(totalLabelRect, totalLabel);
            Text.Font = default;

            TotalHeight = totalLabelRect.yMax - mainRect.y;
            //
        }
    }
}