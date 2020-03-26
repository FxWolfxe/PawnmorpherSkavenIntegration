// JobDriver_TakeToAlter.cs created by Iron Wolf for PMSkaven on 03/26/2020 5:07 PM
// last updated 03/26/2020  5:07 PM

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PMSkaven.AI
{
    public class JobDriver_TakeToAlter : JobDriver
    {
        private const TargetIndex TakeeIndex = TargetIndex.A;

        private const TargetIndex AlterIndex = TargetIndex.B;

        protected Pawn Takee => (Pawn)job.GetTarget(TakeeIndex).Thing;


        protected Building_SkavenAlter Alter => (Building_SkavenAlter) job.GetTarget(AlterIndex); 

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true; 
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TakeeIndex);
            this.FailOnDestroyedOrNull(AlterIndex);
            this.FailOnAggroMentalStateAndHostile(TakeeIndex);
         
            yield return Toils_Goto.GotoThing(TakeeIndex, PathEndMode.ClosestTouch)
                                   .FailOnDespawnedNullOrForbidden(TakeeIndex)
                                   .FailOnDespawnedNullOrForbidden(AlterIndex)
                                   .FailOn(() => !pawn.CanReach(Alter, PathEndMode.OnCell, Danger.Deadly));

            Toil toil2 = Toils_Haul.StartCarryThing(TakeeIndex);
            yield return toil2;
            yield return Toils_Goto.GotoThing(AlterIndex, PathEndMode.Touch);
          
            Toil toil4 = new Toil();

            void DropTarget()
            {
                IntVec3 position = Alter.Position;
                pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out Thing target);

                try
                {
                    var pTarget = (Pawn) target;
                    if (pTarget == null) return;

                    var hediff = HediffMaker.MakeHediff(HediffDefOf.Anesthetic, pTarget);
                    //hacky, but will stop the target from moving around 
                    pTarget.health.hediffSet.AddDirect(hediff); 

                    //now add the transformation hediff 
                    var tfHediff = HediffMaker.MakeHediff(PSHediffDefOf.SkavenRitualEffect, pTarget);
                    pTarget.health.hediffSet.AddDirect(tfHediff); 

                }
                catch (Exception e)
                {
                    Log.Error($"caught {e.GetType().Name} while finishing TakeToAlter action!\n{e} ");
                }
            }

            toil4.initAction = DropTarget;
            toil4.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil4;
        }

        
    }
}