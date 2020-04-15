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


        private const string CALLED_OFF_LABEL = "SkavenTFRitualCalledOffLabel";
        private const string CALLED_OFF_DESCRIPTION = "SkavenTFRitualCalledOffDescription";
        private const string GOOD_END_DESCRIPTION = "SkavenTFRitualGoodEndDescription";
        private const string BAD_END_DESCRIPTION = "SkavenTFRitualBadEndDescription";
        private const string GOOD_END_LABEL = "SkavenTFRitualGoodEndLabel";
        private const string BAD_END_LABEL = "SkavenTFRitualBadEndLabel";
        public Pawn target;
        public LordJob_TfRitual(IntVec3 gatherSpot,  Pawn organizer,  Pawn target, GatheringDef gatheringDef) : base(gatherSpot, organizer,
                                                                                                      gatheringDef)
        {
            this.target = target;
        }

        public Building_SkavenAlter Alter => spot.GetFirstThing<Building_SkavenAlter>(Map);


        
        public IntVec3 GetSpectateCellForPawn(Pawn pawn)
        {
            var cells = Alter.PeripheralCells;
            var idx = lord.ownedPawns.FindIndex(p => p == pawn);
            if (idx < 0)
            {
                Log.Warning($"unable to find cell for non owned pawn {pawn.Name}");
                return IntVec3.Invalid; 
            }

            idx = idx % cells.Count;
            return cells[idx]; 
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

        protected override bool ShouldBeCalledOff()
        {
            return base.ShouldBeCalledOff() || (lord.ticksInToil > 16000 && lord.ownedPawns.Count < 3);
        }

        public override void Notify_PawnAdded(Pawn p)
        {
            base.Notify_PawnAdded(p);
        }

        public override void Notify_PawnLost(Pawn p, PawnLostCondition condition)
        {
            base.Notify_PawnLost(p, condition);
        }

        TaggedString TranslateString(string str, bool capitalizeFirst=true)
        {
            var s = str.Translate(organizer.Named(nameof(organizer)), target.Named(nameof(target)));
            if (capitalizeFirst) s = s.CapitalizeFirst();
            return s; 
        }
#if false
        

        public override void LordJobTick()
        {
            base.LordJobTick();


            var alter = spot.GetFirstThing<Building_SkavenAlter>(Map);
            if (alter == null) return;

            Rand.PushState(alter.thingIDNumber);
            try
            {

                foreach (IntVec3 cell in alter.PeripheralCells)
                {
                    var pos = GenThing.TrueCenter(cell, alter.Rotation, alter.RotatedSize, alter.def.Altitude);
                    CellRenderer.RenderSpot(pos, Rand.Range(0,1f));
                }
            }
            finally
            {
                Rand.PopState();
            }



        }
#endif

        protected override void ApplyOutcome(float progress)
        {
            if (progress < 0.5)
            {
                Find.LetterStack.ReceiveLetter(TranslateString(CALLED_OFF_LABEL),
                                               TranslateString(CALLED_OFF_DESCRIPTION),
                                               LetterDefOf.NegativeEvent, organizer);
            }
            else
            {
                //TODO figure out what causes a 'bad' ending 
                var goodEvent = true;

                TaggedString text = goodEvent
                    ? TranslateString(GOOD_END_DESCRIPTION, false)
                    : TranslateString(BAD_END_DESCRIPTION, false);

                TaggedString label = goodEvent
                    ? TranslateString(GOOD_END_LABEL)
                    : TranslateString(BAD_END_LABEL);

                Find.LetterStack.ReceiveLetter(label, text,
                                               goodEvent ? LetterDefOf.PositiveEvent : LetterDefOf.NegativeEvent,
                                               new LookTargets(new []{organizer,target}));
            }

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