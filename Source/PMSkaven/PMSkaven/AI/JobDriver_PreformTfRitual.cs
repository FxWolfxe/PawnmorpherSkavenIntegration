// JobDriver_PreformTfRitual.cs created by Iron Wolf for PMSkaven on 03/26/2020 4:44 PM
// last updated 03/26/2020  4:44 PM

using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PMSkaven.AI
{
    public class JobDriver_PreformTfRitual : JobDriver
    {
        private const TargetIndex AlterIndex = TargetIndex.A;

        private const TargetIndex FacingIndex = TargetIndex.C;
        private const TargetIndex TakeeIndex = TargetIndex.B;
        protected Building_SkavenAlter Alter => (Building_SkavenAlter)job.GetTarget(AlterIndex);


        private static readonly IntRange MoteInterval = new IntRange(300, 500);

        public static readonly Texture2D moteIcon = ContentFinder<Texture2D>.Get("Things/Mote/SpeechSymbols/Speech");

        private Building_SkavenAlter Throne => (Building_SkavenAlter)base.TargetThingA;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            var reserved =  pawn.Reserve(Throne, job, 1, -1, null, errorOnFailed);
            if (!reserved)
            {
                Log.Message("cannot reserve alter");
            }

            return reserved; 
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

            job.count = 1; 
            Toil toil2 = Toils_Haul.StartCarryThing(TakeeIndex);
            yield return toil2;
            yield return Toils_Goto.GotoThing(AlterIndex, PathEndMode.Touch);

           

            void DropTarget()
            {
                IntVec3 position = Alter.Position;
                pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out Thing target);

                try
                {
                    var pTarget = (Pawn)target;
                    if (pTarget == null) return;

                   
                    pTarget.jobs.StopAll(true, false);


                    //now add the transformation hediff 
                    var tfHediff = HediffMaker.MakeHediff(PSHediffDefOf.SkavenRitualEffect, pTarget);
                    pTarget.health.hediffSet.AddDirect(tfHediff);
                    pTarget.health.CheckForStateChange(null, tfHediff); 
                }
                catch (Exception e)
                {
                    Log.Error($"caught {e.GetType().Name} while finishing TakeToAlter action!\n{e} ");
                }
            }

            Toil toil4 = new Toil {initAction = DropTarget, defaultCompleteMode = ToilCompleteMode.Instant};
            yield return toil4;

            this.FailOnDespawnedNullOrForbidden(AlterIndex);
            yield return Toils_General.Do(delegate
            {
                job.SetTarget(FacingIndex, Throne.InteractionCell + Throne.Rotation.FacingCell);
            });
            Toil toil = new Toil
            {
                tickAction = delegate
                {
                    pawn.GainComfortFromCellIfPossible();
                    pawn.skills.Learn(SkillDefOf.Social, 0.3f);
                    if (pawn.IsHashIntervalTick(MoteInterval.RandomInRange))
                    {
                        MoteMaker.MakeSpeechBubble(pawn, moteIcon);
                    }

                    rotateToFace = FacingIndex;
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };



            //toil.FailOnCannotTouch(ThroneIndex, PathEndMode.InteractionCell);
            yield return toil;
        }
    }
}