public class InteriorSlot
{
    public InteriorData data;
    public RunTimeInteriorData runtimeData;

    public InteriorSlot(InteriorData d, RunTimeInteriorData r)
    {
        data = d;
        runtimeData = r;
        runtimeData.interiorName = d.interiorName;
    }
}
