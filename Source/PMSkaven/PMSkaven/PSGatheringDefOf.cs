// PSGatheringDefOf.cs created by Iron Wolf for PMSkaven on 04/14/2020 2:10 PM
// last updated 04/14/2020  2:10 PM

using RimWorld;

namespace PMSkaven
{
    [DefOf]
    public static class PSGatheringDefOf
    {
        static PSGatheringDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GatheringDef));
        }

        public static GatheringDef SkavenTFGathering; 
    }
}