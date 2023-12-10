using Verse;

namespace TR;

public class TRThing : TRThingPrototype
{
    public WorldComponent_TR TiberiumRimComp => Find.World.GetComponent<WorldComponent_TR>();
    public MapComponent_Tiberium TiberiumMapComp => MapHeld.Tiberium();
}