// JobDriver_SpectateRitual.cs created by Iron Wolf for PMSkaven on 03/29/2020 10:14 AM
// last updated 03/29/2020  10:14 AM

using System.Collections.Generic;
using PMSkaven.Gatherings;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PMSkaven.AI
{
    public class JobDriver_SpectateRitual : JobDriver
    {



        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true; 
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            var ritual = pawn.GetLord()?.LordJob as LordJob_TfRitual;
            if (ritual == null)
            {
                Log.Error($"trying to do {nameof(JobDriver_SpectateRitual)} while {pawn.Name} is not part of the ritual!");
                yield break;
            }

            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell); 
            var toil = new Toil()
            {
                tickAction = () => 
                {
                    pawn.rotationTracker.FaceCell(job.GetTarget(TargetIndex.B).Cell);
                    if (pawn.IsHashIntervalTick(100))
                    {
                        pawn.jobs.CheckForJobOverride();
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never,
                handlingFacing = true
            };
            yield return toil; 

        }



    }
}