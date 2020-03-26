// LordJob_Ritual.cs created by Iron Wolf for PMSkaven on 03/26/2020 3:22 PM
// last updated 03/26/2020  3:22 PM

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace PMSkaven.Gatherings
{
    public class LordJob_TfRitual : LordJob_Joinable_Speech
    {
        public Pawn target; 

        public LordJob_TfRitual(IntVec3 gatherSpot, Pawn organizer,  Pawn target, GatheringDef gatheringDef) : base(gatherSpot, organizer,
                                                                                                      gatheringDef)
        {
            this.target = target; 
        }

        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            LordToil lordToil = CreateGatheringToil(spot, organizer, gatheringDef);
            stateGraph.AddToil(lordToil);
            var lordToil_End = new LordToil_End();
            stateGraph.AddToil(lordToil_End);
            var speechDuration = 12500f;
            var transition = new Transition(lordToil, lordToil_End);
            transition.AddTrigger(new Trigger_TickCondition(ShouldBeCalledOff));
            transition.AddTrigger(new Trigger_PawnKilled());
            transition.AddTrigger(new Trigger_PawnLost(PawnLostCondition.LeftVoluntarily, organizer));
            transition.AddPreAction(new TransitionAction_Custom((Action) delegate
            {
                ApplyOutcome(lord.ticksInToil / speechDuration);
            }));
            stateGraph.AddTransition(transition);
            timeoutTrigger = new Trigger_TicksPassedAfterConditionMet((int) speechDuration,
                                                                      () => GatheringsUtility.InGatheringArea(organizer.Position,
                                                                                                              spot,
                                                                                                              organizer.Map), 60);
            var transition2 = new Transition(lordToil, lordToil_End);
            transition2.AddTrigger(timeoutTrigger);
            transition2.AddPreAction(new TransitionAction_Custom((Action) delegate { ApplyOutcome(1f); }));
            stateGraph.AddTransition(transition2);
            return stateGraph;
        }


        protected override void ApplyOutcome(float progress)
        {
            base.ApplyOutcome(progress);

            if (progress > 0.5f && target != null && target.Faction != Faction.OfPlayer)
            {
                var hediff = target.health.hediffSet.GetFirstHediffOfDef(PSHediffDefOf.SkavenRitualEffect);
                if (hediff != null)
                {
                    hediff.Severity = 0.6f; 
                }

                target.SetFaction(Faction.OfPlayer); 
            }

        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref target, nameof(target)); 
        }

        protected override LordToil CreateGatheringToil(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
        {
            return new LordToil_TfRitual(spot, gatheringDef, organizer);
        }

        
    }
}