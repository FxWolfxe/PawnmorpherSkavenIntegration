// Worker_TfRitual.cs created by Iron Wolf for PMSkaven on 03/26/2020 2:35 PM
// last updated 03/26/2020  2:35 PM

using System;
using System.Collections.Generic;
using System.Linq;
using Pawnmorph;
using Pawnmorph.Utilities;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace PMSkaven.Gatherings
{
    public class Worker_TfRitual : GatheringWorker_Speech
    {
        public const int PARTICIPANT_COUNT = 4;

        public override bool CanExecute(Map map, Pawn organizer = null)
        {
            bool any = PawnsFinder.AllMaps_PrisonersOfColony.Any(IsValidTarget);
            if (!any) Log.Message("unable to find suitable prisoner");
            return base.CanExecute(map, organizer)
                && any
                && map.mapPawns.FreeColonists.Count(IsValidParticipant) >= PARTICIPANT_COUNT;
        }

        bool IsValidParticipant(Pawn pawn)
        {
            if (!pawn.IsColonist) return false;
            return pawn.def == PSThingDefOf.Alien_Skaven; 
        }
        
        public override bool TryExecute(Map map, Pawn organizer = null)
        {
            if (organizer == null)
                organizer = FindOrganizer(map);
            IntVec3 spot;
            if (organizer == null || !TryFindGatherSpot(organizer, out spot))
                return false;

            Pawn target = PawnsFinder.AllMaps_PrisonersOfColony.Where(IsValidTarget).FirstOrDefault();
            if (target == null) return false;

            LordJob lordJob1 = CreateLordJob(spot, organizer, target);
            Faction faction = organizer.Faction;
            LordJob lordJob2 = lordJob1;
            Map map1 = organizer.Map;
            Pawn[] pawnArray;
            if (!lordJob1.OrganizerIsStartingPawn)
                pawnArray = null;
            else
                pawnArray = new Pawn[1] {organizer};
            LordMaker.MakeNewLord(faction, lordJob2, map1, pawnArray);
            SendLetter(spot, organizer, target);
            return true;
        }

        protected LordJob CreateLordJob(IntVec3 spot, Pawn organizer, Pawn target)
        {
            try
            {
                return new LordJob_TfRitual(spot, organizer, target, def);
            }
            catch (Exception e)
            {
                throw new
                    LordInitFailedException($"unable to create {nameof(LordJob_TfRitual)} with organizer:{organizer.Name} at {spot}!",
                                            e);
            }
        }

        protected override Pawn FindOrganizer(Map map)
        {
            Pawn p = base.FindOrganizer(map);
            if (p == null) Log.Message("Unable to find organizer");

            return p;
        }

        protected void SendLetter(IntVec3 spot, Pawn organizer, Pawn target)
        {
            string letterLabel = def.letterTitle;


            TaggedString letterText = def.letterText.Formatted(
                                                               organizer.Named(nameof(organizer)),
                                                               target.Named(nameof(target)));
            Find.LetterStack.ReceiveLetter( letterLabel, letterText, LetterDefOf.PositiveEvent,
                                           new TargetInfo(spot, organizer.Map));

        }


        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            GatherSpotLister gatherSpot = Find.CurrentMap?.gatherSpotLister;

            if (gatherSpot == null)
            {
                spot = default;
                return false;
            }

            foreach (CompGatherSpot compGatherSpot in gatherSpot.activeSpots.MakeSafe())
                if (compGatherSpot.parent is Building_SkavenAlter)
                {
                    ThingWithComps p = compGatherSpot.parent;
                    spot = p.InteractionCell;
                    return true;
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