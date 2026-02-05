using HarmonyLib;
using RimWorld;
using Verse;

namespace CerebrexRebalance
{
    /// <summary>
    /// Triggers quest when Cerebrex Node is first equipped.
    /// Quest provides coordinates to ancient mechanoid complex with Signal Amplifier.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Wear")]
    public static class ArchotechQuestGiver_Patch
    {
        // Track if quest has been given this playthrough
        private static bool questGiven = false;

        [HarmonyPostfix]
        public static void Postfix(Apparel newApparel, Pawn ___pawn)
        {
            if (newApparel == null || ___pawn == null)
            {
                return;
            }

            // Check if it's Cerebrex Node
            if (newApparel.def?.defName != "Apparel_CerebrexNode")
            {
                return;
            }

            // Only trigger once per game
            if (questGiven)
            {
                return;
            }

            // Check if quest already exists
            if (HasArchotechQuest())
            {
                questGiven = true;
                return;
            }

            // Generate the quest
            TryGenerateArchotechQuest(___pawn);
        }

        private static bool HasArchotechQuest()
        {
            if (Find.QuestManager?.QuestsListForReading == null)
            {
                return false;
            }

            foreach (var quest in Find.QuestManager.QuestsListForReading)
            {
                if (quest?.root?.defName == "Cerebrex_RelayCoordinatesQuest")
                {
                    return true;
                }
            }

            return false;
        }

        private static void TryGenerateArchotechQuest(Pawn pawn)
        {
            try
            {
                // Find quest script def
                QuestScriptDef questDef = DefDatabase<QuestScriptDef>.GetNamedSilentFail("Cerebrex_RelayCoordinatesQuest");

                if (questDef == null)
                {
                    Log.Warning("[CerebrexRebalance] Could not find quest def Cerebrex_RelayCoordinatesQuest");
                    return;
                }

                // Calculate threat points based on colony wealth
                float points = StorytellerUtility.DefaultThreatPointsNow(Find.CurrentMap);

                // Generate and make available
                Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(questDef, points);

                if (quest != null)
                {
                    questGiven = true;

                    // Notify player
                    Find.LetterStack.ReceiveLetter(
                        "Archotech Signal Detected",
                        "Equipping the Cerebrex Node has awakened dormant systems. Coordinates for an ancient mechanoid relay station have been transmitted to your colony.",
                        LetterDefOf.PositiveEvent
                    );

                    Log.Message("[CerebrexRebalance] Archotech quest generated successfully");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[CerebrexRebalance] Failed to generate quest: {ex.Message}");
            }
        }

        /// <summary>
        /// Reset quest state for new game.
        /// </summary>
        public static void ResetQuestState()
        {
            questGiven = false;
        }
    }

    /// <summary>
    /// Reset quest state when starting new game.
    /// </summary>
    [HarmonyPatch(typeof(Game), "InitNewGame")]
    public static class Game_InitNewGame_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ArchotechQuestGiver_Patch.ResetQuestState();
        }
    }

    /// <summary>
    /// Reset quest state when loading game (check for existing quest).
    /// </summary>
    [HarmonyPatch(typeof(Game), "LoadGame")]
    public static class Game_LoadGame_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ArchotechQuestGiver_Patch.ResetQuestState();
        }
    }
}
