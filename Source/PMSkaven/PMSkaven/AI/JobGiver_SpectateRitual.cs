// JobGiver_SpectateRitual.cs created by Iron Wolf for PMSkaven on 03/29/2020 10:05 AM
// last updated 03/29/2020  10:05 AM

using PMSkaven.Gatherings;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PMSkaven.AI
{
    public class JobGiver_SpectateRitual : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var duty = pawn.mindState.duty; 
            if(duty == null)
            {
                return null; 
            }

            var lord = pawn.GetLord();
            var lordJob = lord?.LordJob as LordJob_TfRitual;
            if (lordJob == null) return null;

            var index = lord.ownedPawns.IndexOf(pawn);
            if (index == -1 || index >= Worker_TfRitual.PARTICIPANT_COUNT)
            {
                return null; 
            }

            var alter = duty.focusSecond.Thing as Building_SkavenAlter;
            if (alter == null) return null;
            var cell = alter.PeripheralCells[index];

            
            return JobMaker.MakeJob(PSJobDefOf.SpectateRitual, cell, alter.InteractionCell + new IntVec3(0, -1, 0)); 
        }
    }
}