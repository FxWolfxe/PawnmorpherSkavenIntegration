// ThinkNode_ConditionOneOfEleven.cs created by Iron Wolf for PMSkaven on 03/29/2020 10:20 AM
// last updated 03/29/2020  10:20 AM

using PMSkaven.Gatherings;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PMSkaven.AI
{
    public class ThinkNode_ConditionOneOfEleven : ThinkNode_Conditional
    {
        

        protected override bool Satisfied(Pawn pawn)
        {
            Lord lord = pawn.GetLord();
            var lordJob = lord?.LordJob as LordJob_TfRitual;
            if (lordJob == null) return false;

            if (lordJob.Organizer == pawn) return true;

            //more then 11 skaven can join 
            //the extra ones 
            var index = lord.ownedPawns.IndexOf(pawn);
            return index != -1 && index < PMSkavenMod.RequiredNumberOfSkaven; 


        }
    }
}