using System.Collections.Generic;
using RimWorld;
using TeleCore.RWExtended;
using Verse;

namespace TR;

public class TRBuilding : TRBuildingPrototype
{
    public WorldComponent_TR TiberiumRimComp = Find.World.GetComponent<WorldComponent_TR>();
    public MapComponent_Tiberium TiberiumComp => Map.Tiberium();
    
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        TiberiumRimComp.Notify_BuildingSpawned(this);
        TiberiumComp.RegisterTRBuilding(this);
        foreach (IntVec3 c in this.OccupiedRect())
        {
            if (def.clearTiberium)
                c.GetTiberium(Map)?.DeSpawn();
        }
    }
    
    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        TiberiumComp.DeregisterTRBuilding(this);
        base.DeSpawn(mode);
    }
}