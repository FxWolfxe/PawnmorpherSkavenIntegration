// PMSkavenMod.cs created by Iron Wolf for PMSkaven on 03/24/2020 7:21 AM
// last updated 03/24/2020  7:21 AM

using UnityEngine;
using Verse;

namespace PMSkaven
{
    public class PMSkavenMod : Mod
    {
        public SkavenIntegrationSettings settings;

        public static int RequiredNumberOfSkaven
        {
            get { return LoadedModManager.GetMod<PMSkavenMod>().settings.requiredSkaven;  }
        }

        public static bool AnyRaceCanParticipate
        {
            get { return LoadedModManager.GetMod<PMSkavenMod>().settings.anyRaceCanParticipate; }
        }


        public static bool AnyRaceCanLead
        {
            get
            {
                var modSettings = LoadedModManager.GetMod<PMSkavenMod>().settings;
                return modSettings.anyRaceCanParticipate && modSettings.anyRaceCanLead; 
            }
        }

        public PMSkavenMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<SkavenIntegrationSettings>(); 
        }

        private const string REQUIRED_SKAVEN = "PMS_RequiredSkavenNumber";
        private const string ANY_RACE_PARTICIPATE = "PMS_AnyRaceCanParticipate";
        private const string ANY_RACE_LEAD = "PMS_AnyRaceCanLead";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var ls = new Listing_Standard(); 
            ls.Begin(inRect);
            try
            {
                ls.Label($"{REQUIRED_SKAVEN.Translate()}:{settings.requiredSkaven}");
                settings.requiredSkaven = (int) ls.Slider(settings.requiredSkaven, 1, 12);
                ls.CheckboxLabeled(ANY_RACE_PARTICIPATE.Translate(), ref settings.anyRaceCanParticipate);
                if (settings.anyRaceCanParticipate)
                {
                    ls.CheckboxLabeled(ANY_RACE_LEAD.Translate(), ref settings.anyRaceCanLead);
                }
            }
            finally
            {
                ls.End();
            }

        }
    }

    public class SkavenIntegrationSettings : ModSettings
    {
        public int requiredSkaven = 11;
        public bool anyRaceCanParticipate = false;
        public bool anyRaceCanLead = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref requiredSkaven, nameof(requiredSkaven), 11); 
            Scribe_Values.Look(ref anyRaceCanLead, nameof(anyRaceCanLead));
            Scribe_Values.Look(ref anyRaceCanParticipate, nameof(anyRaceCanParticipate));
        }
    }
}