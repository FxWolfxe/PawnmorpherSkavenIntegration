// PSJobDefOf.cs created by Iron Wolf for PMSkaven on 03/26/2020 4:46 PM
// last updated 03/26/2020  4:46 PM

using RimWorld;
using Verse;

namespace PMSkaven
{
    [DefOf]
    public static class PSJobDefOf
    {
        static PSJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDef));
        }

        public static JobDef PreformTfRitual; 
    }
}