// PSDutyDefOf.cs created by Iron Wolf for PMSkaven on 03/26/2020 3:27 PM
// last updated 03/26/2020  3:27 PM

using RimWorld;
using Verse.AI;

namespace PMSkaven
{
    
    [DefOf]
    public static class PSDutyDefOf
    {
        static PSDutyDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DutyDef)); 
        }

        public static DutyDef PerformTfRitual;

        public static DutyDef SpectateRitual; 
    }
}