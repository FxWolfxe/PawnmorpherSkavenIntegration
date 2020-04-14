// Building_SkavenAlter.cs created by Iron Wolf for PMSkaven on 03/26/2020 3:36 PM
// last updated 03/26/2020  3:36 PM

using System;
using System.Collections.Generic;
using System.Linq;
using PMSkaven.Gatherings;
using RimWorld;
using Verse;

namespace PMSkaven
{
    public class Building_SkavenAlter : Building
    {
        private IntVec3[] _peripheralCells;

        public IReadOnlyList<IntVec3> PeripheralCells
        {
            get
            {
                if (_peripheralCells == null)
                {
                    _peripheralCells = new IntVec3[12];

                    //interact cell is the top center cell of the table 
                    IntVec3 iCel = InteractionCell;


                    //top cells 

                    for (var i = 0; i < 3; i++)
                    {
                        int x = i - 1;
                        _peripheralCells[i] = iCel + new IntVec3(x, 0, 1);
                    }

                    //right side 
                    for (var i = 0; i < 3; i++)
                    {
                        int index = i + 3;
                        int y = 0 - i;
                        _peripheralCells[index] = iCel + new IntVec3(2, 0, y);
                    }

                    //bottom 
                    for (var i = 0; i < 3; i++)
                    {
                        int index = i + 6;
                        int x = i - 1;
                        _peripheralCells[index] = iCel + new IntVec3(x, 0, -3);
                    }

                    //left
                    for (var i = 0; i < 3; i++)
                    {
                        int index = i + 9;
                        int y = 0 - i;
                        _peripheralCells[index] = iCel + new IntVec3(-2, 0, y);
                    }
                }

                return _peripheralCells;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            //TODO Graphics 
            foreach (Gizmo gizmo in base.GetGizmos()) yield return gizmo;

            if (Faction != Faction.OfPlayer) yield break;
            if (Map == null) yield break;

            var cmd = new Command_Action
            {
                action = StartRitual
            };
            List<Pawn> skaven = GetAllLeaders().ToList();
            GatheringDef gDef = PSGatheringDefOf.SkavenTFGathering;

            try
            {
                var worker = (Worker_TfRitual) gDef.Worker;


                if (!worker.CanExecute(Map, skaven.FirstOrDefault(), out string reason)) cmd.Disable(reason);
            }
            catch (InvalidCastException e)
            {
                Log.Error($"unable to cast worker on {gDef.defName} to {nameof(Worker_TfRitual)}!\n{e.ToString().Indented("|\t")}");
                yield break;
            }

            yield return cmd;
        }

        private IEnumerable<Pawn> GetAllLeaders()
        {
            foreach (Pawn pawn in Map.listerThings.ThingsOfDef(PSThingDefOf.Alien_Skaven).OfType<Pawn>())
            {
                if (pawn.Faction != Faction.OfPlayer || pawn.Downed || pawn.Dead || !pawn.Spawned) continue;
                //TODO check if the skaven is a valid 'leander'
                yield return pawn;
            }
        }

        private void StartRitual()
        {
            Pawn goodSkaven = GetAllLeaders().FirstOrDefault();
            if (goodSkaven == null)
            {
                Log.Warning("could not start ritual because there is no valid leader!");
                return;
            }

            GatheringDef gDef = PSGatheringDefOf.SkavenTFGathering;
            if (!gDef.Worker.CanExecute(Map, goodSkaven))
                Log.Warning($"unable to start ritual because skaven returned by {nameof(GetAllLeaders)} is not a valid leader!");

            if (!gDef.Worker.TryExecute(Map, goodSkaven)) Log.Warning($"Could not start ritual with {goodSkaven.Name} as leader");
        }
    }
}