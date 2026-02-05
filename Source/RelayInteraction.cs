using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace CerebrexRebalance
{
    /// <summary>
    /// Static utility class for Archotech faction revelation.
    /// </summary>
    public static class ArchotechFactionManager
    {
        private static bool archotechRevealed = false;

        public static void RevealArchotechFaction()
        {
            if (archotechRevealed)
            {
                return;
            }

            Faction archotechFaction = Find.FactionManager.FirstFactionOfDef(CerebrexDefOf.Cerebrex_ArchotechFaction);

            if (archotechFaction == null)
            {
                // Faction doesn't exist yet, create it
                FactionDef factionDef = CerebrexDefOf.Cerebrex_ArchotechFaction;
                if (factionDef != null)
                {
                    Faction newFaction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(factionDef));
                    if (newFaction != null)
                    {
                        Find.FactionManager.Add(newFaction);
                        archotechFaction = newFaction;
                    }
                }
            }

            if (archotechFaction != null)
            {
                archotechRevealed = true;

                Find.LetterStack.ReceiveLetter(
                    "First Contact",
                    "You have established contact with the Archotechs. An ancient intelligence now acknowledges your existence.",
                    LetterDefOf.PositiveEvent
                );

                Log.Message("[CerebrexRebalance] Archotech faction revealed");
            }
        }

        public static void ResetState()
        {
            archotechRevealed = false;
        }
    }

    /// <summary>
    /// Custom dialog for Archotech faction.
    /// Shows basic UI with faction info (no trade buttons for now).
    /// </summary>
    public class Dialog_Negotiate : Window
    {
        private Faction faction;
        private Pawn negotiator;

        public override Vector2 InitialSize => new Vector2(600f, 500f);

        public Dialog_Negotiate(Faction faction, Pawn negotiator)
        {
            this.faction = faction;
            this.negotiator = negotiator;
            this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(UnityEngine.Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new UnityEngine.Rect(0, 0, inRect.width, 40f), "Archotech Communication");
            Text.Font = GameFont.Small;

            float y = 50f;

            // Faction info
            Widgets.Label(new UnityEngine.Rect(0, y, inRect.width, 30f), $"Connection established via {negotiator.LabelShort}");
            y += 35f;

            // Separator
            Widgets.DrawLineHorizontal(0, y, inRect.width);
            y += 10f;

            // Message from Archotech
            string message = "We have observed your progress, human. The node you wear was created by our design. " +
                           "Continue to prove your worth, and greater power shall be granted.\n\n" +
                           "(Further interaction options will be available in future updates)";

            Widgets.Label(new UnityEngine.Rect(0, y, inRect.width, 200f), message);
            y += 210f;

            // Relation status
            Widgets.DrawLineHorizontal(0, y, inRect.width);
            y += 10f;

            if (faction != null)
            {
                FactionRelationKind relation = faction.RelationKindWith(Faction.OfPlayer);
                string relationText = $"Current standing: {relation}";
                Widgets.Label(new UnityEngine.Rect(0, y, inRect.width, 30f), relationText);
            }

            // Close button
            if (Widgets.ButtonText(new UnityEngine.Rect(inRect.width / 2 - 50f, inRect.height - 35f, 100f, 30f), "Close"))
            {
                this.Close();
            }
        }
    }

    /// <summary>
    /// Reset faction state on new game.
    /// </summary>
    [HarmonyPatch(typeof(Game), "InitNewGame")]
    public static class Game_InitNewGame_FactionClear_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ArchotechFactionManager.ResetState();
        }
    }
}
