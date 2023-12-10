using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TR;

[StaticConstructorOnStartup]
public static class StaticData
{
    public static Dictionary<int, MapComponent_Tiberium> TiberiumMapComp;

    public static Dictionary<int, Color[]> CanvasBySize;

    //TR Build Menu
    private static Dictionary<ThingDef, Designator> CachedDesignators;

    //
    private static Dictionary<string, object> MP;


    static StaticData()
    {
        Notify_Reload();
    }

    public static void Notify_Reload()
    {
        CanvasBySize = new Dictionary<int, Color[]>();
        TiberiumMapComp = new Dictionary<int, MapComponent_Tiberium>();
        CachedDesignators = new Dictionary<ThingDef, Designator>();
    }

    public static void Notify_ClearingMapAndWorld()
    {
        TRFind.TickManager.ClearGameTickers();
    }

    public static void Notify_NewTibMapComp(MapComponent_Tiberium mapComp)
    {
        mapComp.map.GetComponent<MapComponent_Tiberium>();
        TiberiumMapComp[mapComp.map.uniqueID] = mapComp;
    }

    //Data Accessors
    public static T GetDesignatorFor<T>(ThingDef def) where T : Designator
    {
        if (CachedDesignators.TryGetValue(def, out var des))
        {
            return (T)des;
        }

        des = (Designator)Activator.CreateInstance(typeof(T), def);
        des.icon = def.uiIcon;
        CachedDesignators.Add(def, des);
        return (T)des;
    }
}