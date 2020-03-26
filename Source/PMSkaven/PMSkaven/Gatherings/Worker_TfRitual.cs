// Worker_TfRitual.cs created by Iron Wolf for PMSkaven on 03/26/2020 2:35 PM
// last updated 03/26/2020  2:35 PM

using System;
using System.Linq;
using Pawnmorph;
using Pawnmorph.Utilities;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace PMSkaven.Gatherings
{
    public class Worker_TfRitual : GatheringWorker_Speech
    {
        public override bool CanExecute(Map map, Pawn organizer = null)
        {
            bool any = PawnsFinder.AllMaps_PrisonersOfColony.Any(IsValidTarget);
            if (!any)
            {
                Log.Message("unable to find suitable prisoner"); 
            }
            return base.CanExecute(map, organizer)
                && any;
        }

        protected override Pawn FindOrganizer(Map map)
        {
            var p = base.FindOrganizer(map);
            if (p == null)
            {
                Log.Message("Unable to find organizer");
            }

            return p; 
        }

        protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer)
        {
            try
            {
                var target = PawnsFinder.AllMaps_PrisonersOfColony.First(IsValidTarget);

                return new LordJob_TfRitual(spot, organizer, target, this.def);
            }
            catch (Exception e)
            {
                throw new
                    LordInitFailedException($"unable to create {nameof(LordJob_TfRitual)} with organizer:{organizer.Name} at {spot}!",
                                            e);
            }
        }

     

        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            var gatherSpot = Find.CurrentMap?.gatherSpotLister;

            if (gatherSpot == null)
            {
                spot = default; 
                return false;
            }

            foreach (CompGatherSpot compGatherSpot in gatherSpot.activeSpots.MakeSafe())
            {
                if (compGatherSpot.parent is Building_SkavenAlter)
                {
                    var p = compGatherSpot.parent;
                    spot = p.InteractionCell;
                    return true; 
                }
            }
           
         
            spot = default; 
            return false; 
        }


        private bool IsValidTarget(Pawn pawn)
        {
            return pawn.IsPrisonerOfColony
                && pawn.def != PSThingDefOf.Alien_Skaven
                && MorphUtilities.HybridsAreEnabledFor(pawn.def);
        }
    }
}