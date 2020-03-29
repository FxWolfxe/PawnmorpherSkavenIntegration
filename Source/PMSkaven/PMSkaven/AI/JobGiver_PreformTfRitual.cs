// ThinkNode_PreformTfRitual.cs created by Iron Wolf for PMSkaven on 03/26/2020 4:40 PM
// last updated 03/26/2020  4:40 PM

using PMSkaven.Gatherings;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PMSkaven.AI
{
    public class JobGiver_PreformTfRitual : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            PawnDuty duty = pawn.mindState.duty;

            if (!(pawn.GetLord().LordJob is LordJob_TfRitual ritualLord))
            {
                Log.Message($"pawn is not part of ritual lord");

                return null;
            }

            if (ritualLord.Organizer != pawn)
            {
                Log.Message("pawn is not the organizer");
                return null; 
            }


            if (!(duty?.focusSecond.Thing is Building_SkavenAlter building_Throne))
            {
                Log.Message("target is not alter");
                return null;
            }
            if (!pawn.CanReach(building_Throne, PathEndMode.InteractionCell, Danger.None))
            {
                Log.Message("could not reach alter");
                return null;
            }

            var map = ritualLord.target.Map;
            map.reservationManager.ReleaseAllForTarget(ritualLord.target); 


            return JobMaker.MakeJob(PSJobDefOf.PreformTfRitual, duty.focusSecond, ritualLord.target); 
        }
    }
}