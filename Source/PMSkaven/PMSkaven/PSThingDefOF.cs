// PSThingDefOF.cs created by Iron Wolf for PMSkaven on 03/26/2020 2:36 PM
// last updated 03/26/2020  2:36 PM

using RimWorld;
using Verse;

namespace PMSkaven
{
    [DefOf]
    public static class PSThingDefOf
    {
        public static ThingDef Alien_Skaven;

        static PSThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDef));
        }
    }
}