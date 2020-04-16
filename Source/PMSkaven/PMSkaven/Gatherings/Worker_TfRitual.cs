// Worker_TfRitual.cs created by Iron Wolf for PMSkaven on 03/26/2020 2:35 PM
// last updated 03/26/2020  2:35 PM

using System;
using System.Linq;
using JetBrains.Annotations;
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

        private const string NO_ORGANIZER = "NoHeadSkaven";
        private const string NOT_ENOUGH_SKAVEN = "NotEnoughSkaven";
        private const string NO_VALID_TARGET = "NoValidSkavenTfTarget";

        public override bool CanExecute(Map map, Pawn organizer = null)
        {
            bool any = PawnsFinder.AllMaps_PrisonersOfColony.Any(RitualUtilities.IsValidRitualTarget);

            return base.CanExecute(map, organizer)
                && any
                && map.mapPawns.FreeColonists.Count(RitualUtilities.IsValidRitualParticipant) >= PARTICIPANT_COUNT;
        }

        public bool CanExecute(Map map, [CanBeNull] Pawn organizer, out string reason)
        {
            if (organizer == null)
            {
                reason = NO_ORGANIZER.Translate();
                return false;
            }

            if (!base.CanExecute(map, organizer))
            {
                reason = "unknown";
                return false;
            }

            if (!PawnsFinder.AllMaps_PrisonersOfColony.Any(RitualUtilities.IsValidRitualTarget))
            {
                reason = NO_VALID_TARGET.Translate();
                return false;
            }

            if (map.mapPawns.FreeColonists.Count(RitualUtilities.IsValidRitualParticipant) < PARTICIPANT_COUNT)
            {
                reason = NOT_ENOUGH_SKAVEN.Translate();
                return false;
            }

            reason = "";
            return true;
        }

        public override bool TryExecute(Map map, Pawn organizer = null)
        {
            if (organizer == null)
                organizer = FindOrganizer(map);
            if (organizer == null || !TryFindGatherSpot(organizer, out IntVec3 spot))
                return false;

            Pawn target = PawnsFinder.AllMaps_PrisonersOfColony.Where(RitualUtilities.IsValidRitualTarget).FirstOrDefault();
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
            Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.PositiveEvent,
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

     


    }
}