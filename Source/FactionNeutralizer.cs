using HarmonyLib;
using RimWorld;
using Verse;

namespace CerebrexRebalance
{
    [HarmonyPatch(typeof(Quest), "End")]
    public static class QuestEnd_Patch
    {
        private static bool neutralized = false;

        public static void Postfix(Quest __instance, QuestEndOutcome outcome)
        {
            if (outcome == QuestEndOutcome.Success && !neutralized)
            {
                if (__instance.root != null)
                {
                    bool isMechQuest = __instance.root.defName.Contains("Mech") || __instance.root.defName.Contains("Odyssey");
                    
                    if (isMechQuest)
                    {
                        NeutralizeMechFaction();
                    }
                }
            }
        }

        public static void NeutralizeMechFaction()
        {
            if (neutralized) return;

            Faction mechFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Mechanoid);
            if (mechFaction != null && Faction.OfPlayer != null)
            {
                // Use SetRelationDirect for RimWorld 1.6
                mechFaction.SetRelationDirect(Faction.OfPlayer, FactionRelationKind.Neutral, false);
                Messages.Message("The Mechanoid faction has been neutralized.", MessageTypeDefOf.PositiveEvent);
                neutralized = true;
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Wear")]
    public static class Apparel_Patch
    {
        public static void Postfix(Apparel newApparel)
        {
            if (newApparel != null && newApparel.def != null && newApparel.def.defName == "Apparel_CerebrexNode")
            {
                QuestEnd_Patch.NeutralizeMechFaction();
            }
        }
    }
}
