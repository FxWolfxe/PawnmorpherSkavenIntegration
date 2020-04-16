// RitualUtilities.cs created by Iron Wolf for PMSkaven on 04/15/2020 7:58 PM
// last updated 04/15/2020  7:58 PM

using System;
using JetBrains.Annotations;
using Pawnmorph;
using RimWorld;
using Verse;

namespace PMSkaven
{
    public static class RitualUtilities
    {
        public static bool IsValidRitualParticipant([NotNull] this Pawn pawn)
        {
            if (pawn.Faction != Faction.OfPlayer || pawn.Downed || pawn.Dead || !pawn.Spawned) return false;
            return pawn.def == PSThingDefOf.Alien_Skaven;
        }

        public static bool IsValidRitualLeader([NotNull] this Pawn pawn)
        {
            if (pawn == null) throw new ArgumentNullException(nameof(pawn));
            if (!IsValidRitualParticipant(pawn)) return false;

            //TODO leader qualifications 
            return true;

        }


        public static bool IsValidRitualTarget([NotNull] this Pawn pawn)
        {
            if (pawn == null) throw new ArgumentNullException(nameof(pawn));
            return pawn.IsPrisonerOfColony
                && pawn.def != PSThingDefOf.Alien_Skaven
                && MorphUtilities.HybridsAreEnabledFor(pawn.def);
        }
    }
}