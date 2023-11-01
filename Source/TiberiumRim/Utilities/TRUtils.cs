using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using TeleCore;
using UnityEngine;
using Verse;
using static System.String;

namespace TR;

public static class TRUtils
{
    public static MapComponent_Tiberium Tiberium(this Map map)
    {
        if (map == null)
        {
            TRLog.Warning("Map is null for Tiberium MapComp getter");
            return null;
        }

        return StaticData.TiberiumMapComp[map.uniqueID];
    }

    public static WorldComponent_TR Tiberium()
    {
        return Find.World.GetComponent<WorldComponent_TR>();
    }

    public static GameSettingsInfo GameSettings()
    {
        return Tiberium().GameSettings;
    }
}