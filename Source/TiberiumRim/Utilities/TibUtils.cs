using System;
using System.Collections.Generic;
using RimWorld;
using TeleCore;
using TR.TGraphics;
using UnityEngine;
using Verse;

namespace TR.Utilities;

public static class TibUtils
{
    public static class Network
    {
        public static NetworkValueDef[] MainValueTypes = new[] { TiberiumDefOf.TibGreen, TiberiumDefOf.TibBlue, TiberiumDefOf.TibRed };
    }
    
    public static bool IsTiberiumTerrain(this TerrainDef def)
    {
        return def is TiberiumTerrainDef;
    }

    public static TiberiumCrystalDef CrystalDefFromType(NetworkValueDef valueDef, out bool isGas)
    {
        isGas = false;
        //TODO: Adjust drops
        if (valueDef == TiberiumDefOf.TibGreen)
        {
            return TiberiumDefOf.TiberiumGreen;
        }

        if (valueDef == TiberiumDefOf.TibBlue)
        {
            return TiberiumDefOf.TiberiumBlue;
        }

        if (valueDef == TiberiumDefOf.TibRed)
        {
            return TiberiumDefOf.TiberiumRed;
        }

        if (valueDef == TiberiumDefOf.TibSludge)
        {
            return TiberiumDefOf.TiberiumMossGreen;
        }

        if (valueDef == TiberiumDefOf.TibGas)
        {
            isGas = true;
        }

        return null;
    }
    
    
    public static Texture2D MaterialForType(TiberiumValueType valueType)
    {
        Texture2D tex = SolidColorMaterials.NewSolidColorTexture(Color.black);
        TiberiumControlDef def = MainTCD.Main;
        switch (valueType)
        {
            case TiberiumValueType.Green:
                tex = TiberiumMats.GreenType;
                break;
            case TiberiumValueType.Blue:
                tex = TiberiumMats.BlueType;
                break;
            case TiberiumValueType.Red:
                tex = TiberiumMats.RedType;
                break;
            case TiberiumValueType.Sludge:
                tex = TiberiumMats.SludgeType;
                break;
            case TiberiumValueType.Gas:
                tex = TiberiumMats.GasType;
                break;
        }

        return tex;
    }
    
    public static Color GetColor(this Enum valueType)
    {
        Color color = Color.white;
        if (valueType is TiberiumValueType tibType)
        {
            TiberiumControlDef def = MainTCD.Main;
            color = tibType switch
            {
                TiberiumValueType.Green => def.GreenColor,
                TiberiumValueType.Blue => def.BlueColor,
                TiberiumValueType.Red => def.RedColor,
                TiberiumValueType.Sludge => def.SludgeColor,
                TiberiumValueType.Gas => def.GasColor,
                _ => color
            };
        }

        return color;
    }
    
    public static string ShortLabel(this Enum enumType)
    {
        if (enumType is TiberiumValueType tibVal)
        {
            return ShortLabel(tibVal);
        }

        if (enumType is AtmosphericValueType atmosVal)
        {
            return ShortLabel(atmosVal);
        }

        return string.Empty;
    }
    
    public static string ShortLabel(this AtmosphericValueType valueType)
    {
        string label = valueType switch
        {
            AtmosphericValueType.Air => "Air",
            AtmosphericValueType.Pollution => "Poll",
            _ => String.Empty
        };

        return label;
    }

    public static string ShortLabel(this TiberiumValueType valueType)
    {
        string label = valueType switch
        {
            TiberiumValueType.Green => "G",
            TiberiumValueType.Blue => "B",
            TiberiumValueType.Red => "R",
            TiberiumValueType.Sludge => "Slg",
            TiberiumValueType.Gas => "Gs",
            _ => String.Empty
        };

        return label;
    }
    
    public static void GetTiberiumMutant(Pawn pawn, out Graphic Head, out Graphic Body)
    {
        Head = null;
        Body = null;
        if (pawn.def.defName != "Human")
        {
            PawnGraphicSet graphicSet = pawn.Drawer.renderer.graphics;
            string headPath = graphicSet.headGraphic.path + "_TibHead";
            string bodyPath = graphicSet.nakedGraphic.path + "_TibBody";
            Head = GraphicDatabase.Get(typeof(Graphic_Multi), headPath, ShaderDatabase.Cutout, Vector2.one, Color.white,
                Color.white);
            Body = GraphicDatabase.Get(typeof(Graphic_Multi), bodyPath, ShaderDatabase.Cutout, Vector2.one, Color.white,
                Color.white);
        }
        else
        {
            HeadTypeDef head = pawn.story.headType;
            string headPath = head.graphicPath;
            string headResolved;
            BodyTypeDef body = pawn.story.bodyType;
            string bodyResolved;
            Gender gender = pawn.gender;

            string appendix = "";
            if (headPath.Contains("_Wide"))
            {
                appendix = "_Wide";
            }

            if (headPath.Contains("_Normal"))
            {
                appendix = "_Normal";
            }

            if (headPath.Contains("_Pointy"))
            {
                appendix = "_Pointy";
            }

            headResolved = "Pawns/TiberiumMutant/Heads/" + gender + "_" + head + appendix;
            //Head = GraphicDatabase.Get(typeof(Graphic_Multi), headResolved, ShaderDatabase.MoteGlow, Vector2.one, Color.white, Color.white);
            Head = GraphicDatabase.Get(typeof(Graphic_Multi), "Pawns/TiberiumMutant/Heads/Mutant_head",
                ShaderDatabase.Cutout, Vector2.one, Color.white, Color.white);
            bodyResolved = "Pawns/TiberiumMutant/Bodies/" + body.defName;
            Body = GraphicDatabase.Get(typeof(Graphic_Multi), bodyResolved, ShaderDatabase.Cutout, Vector2.one,
                Color.white, Color.white);
        }
    }
}