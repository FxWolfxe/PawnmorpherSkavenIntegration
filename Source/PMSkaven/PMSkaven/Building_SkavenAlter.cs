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
                action = CreateSubMenu
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

        private FloatMenuOption CreateOption(Pawn p, Pawn leader)
        {
            var worker = (Worker_TfRitual) PSGatheringDefOf.SkavenTFGathering.Worker;
            return new FloatMenuOption(p.Name.ToStringShort, () =>
            {
                try
                {
                    if (!worker.TryExecute(leader, p))
                        Log.Warning($"unable to start ritual with {leader.Name} and target {p.Name}");
                }
                catch (Exception e)
                {
                    Log.Error($"caught {e.GetType().Name} while executing ritual gathering!\n{e.ToString().Indented("|\t")}");
                }
            });
        }


        private void CreateSubMenu()
        {
            Pawn goodSkaven = GetAllLeaders().FirstOrDefault();
            if (goodSkaven == null)
            {
                Log.Warning("could not start ritual because there is no valid leader!");
                return;
            }

            try
            {
                List<FloatMenuOption> options = GetAllTargets().Select(p => CreateOption(p, goodSkaven)).ToList();
                var menu = new FloatMenu(options);
                Find.WindowStack.Add(menu);
            }
            catch (Exception e)
            {
                Log.Error($"caught {e.GetType().Name} while trying to create sub menu!\n{e.ToString().Indented("|\t")}");
            }
        }

        private IEnumerable<Pawn> GetAllLeaders()
        {
            foreach (Pawn pawn in Map.listerThings.ThingsOfDef(PSThingDefOf.Alien_Skaven).OfType<Pawn>())
            {
                if (!pawn.IsValidRitualLeader()) continue;
                yield return pawn;
            }
        }


        private IEnumerable<Pawn> GetAllTargets()
        {
            return Map.mapPawns.AllPawnsSpawned.Where(RitualUtilities.IsValidRitualTarget);
        }
    }
}