// LordToil_TfRitual.cs created by Iron Wolf for PMSkaven on 03/26/2020 3:27 PM
// last updated 03/26/2020  3:27 PM

using RimWorld;
using Verse;
using Verse.AI;

namespace PMSkaven.Gatherings
{
    public class LordToil_TfRitual : LordToil_Speech
    {
        
        public new LordToilData_TfRitual Data => (LordToilData_TfRitual) data; 

        public LordToil_TfRitual(IntVec3 spot,  GatheringDef gatheringDef, Pawn organizer) : base(spot, gatheringDef, organizer)
        {
            this.organizer = organizer;
            data = new LordToilData_TfRitual(); 

        }

        public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
        {
            if (p == organizer)
            {
                return PSDutyDefOf.PerformTfRitual.hook; 
            }

            return DutyDefOf.Spectate.hook; 
        }

        public override void UpdateAllDuties()
        {
            for (var index = 0; index < lord.ownedPawns.Count; index++)
            {
                Pawn pawn = lord.ownedPawns[index];
                if (pawn == organizer)
                {
                    var firstThing = spot.GetFirstThing<Building_SkavenAlter>(base.Map);
                    pawn.mindState.duty = new PawnDuty(PSDutyDefOf.PerformTfRitual, spot, firstThing);
                    pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
                else
                {
                    PawnDuty pawnDuty = new PawnDuty(DutyDefOf.Spectate)
                    {
                        spectateRect = Data.spectateRect, spectateRectAllowedSides = Data.spectateRectAllowedSides
                    };
                    pawn.mindState.duty = pawnDuty;
                }
            }
        }
    }
}