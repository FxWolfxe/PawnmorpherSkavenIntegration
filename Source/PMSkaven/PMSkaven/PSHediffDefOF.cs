// PSHediffDefOF.cs created by Iron Wolf for PMSkaven on 03/26/2020 5:24 PM
// last updated 03/26/2020  5:24 PM

using RimWorld;
using Verse;

namespace PMSkaven
{

    [DefOf]
    public static class PSHediffDefOf
    {
        static PSHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDef));
        }
        public static HediffDef SkavenRitualEffect;
    }
}