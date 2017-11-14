﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace ModifyResearchTime
{
    internal static class ResearchTimeUtil
    {
        private static Dictionary<string, float> baseResearchDefs = null;

        public static void CreateBaseResearchDefs()
        {
            if (baseResearchDefs == null)
            {
#if DEBUG
                Log.Warning("Create Base Research Lookup");
#endif
                baseResearchDefs = new Dictionary<string, float>();
                foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
                {
                    baseResearchDefs.Add(def.defName, def.baseCost);
                }
            }
        }

        public static void ApplyFactor(float factor)
        {
#if DEBUG
            Log.Warning("ApplyFactor");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
#endif
            Dictionary<ResearchProjectDef, float> progress =
                (Dictionary<ResearchProjectDef, float>)Find.ResearchManager.GetType().GetField("progress", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Find.ResearchManager);

            CreateBaseResearchDefs();
            ResetResearchFactor();

            foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
            {
#if DEBUG
                float orig = def.baseCost;
                bool finsihed = def.IsFinished;
#endif
                bool finished = def.IsFinished;
                float p = progress[def];
                progress[def] = p * factor;
                def.baseCost *= factor;
                if (finished && !def.IsFinished)
                {
                    Find.ResearchManager.InstantFinish(def, false);
                }
#if DEBUG
                //sb.Append(def.defName + " Finished Orig: " + finsihed + " New: " + def.IsFinished + " Base Cost Orig: " + (int)orig + " New: " + (int)def.baseCost);
#endif
            }
#if DEBUG
            Log.Warning(sb.ToString());
#endif
        }

        public static void ResetResearchFactor()
        {
#if DEBUG
            Log.Warning("ResetResearchFactor");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
#endif
            if (baseResearchDefs != null)
            {
                foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
                {
#if DEBUG
                    float orig = def.baseCost;
                    bool finsihed = def.IsFinished;
#endif
                    def.baseCost = baseResearchDefs[def.defName];
#if DEBUG
                    //sb.Append(def.defName + " Finished Orig: " + finsihed + " New: " + def.IsFinished + " Base Cost Orig: " + (int)orig + " New: " + (int)def.baseCost);
#endif
                }
            }
#if DEBUG
            Log.Warning(sb.ToString());
#endif
        }
    }
}
