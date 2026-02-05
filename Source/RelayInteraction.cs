using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace CerebrexRebalance
{
    /// <summary>
    /// Handles interaction with Archotech Relay building.
    /// Only colonists with Cerebrex Node can use it.
    /// Reveals hidden Archotech faction on first use.
    /// </summary>
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class RelayInteraction_Patch
    {
        private static bool archotechRevealed = false;

        [HarmonyPostfix]
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            if (pawn == null || opts == null || pawn.Map == null)
            {
                return;
            }

            // Find Relay building at click position
            IntVec3 cell = IntVec3.FromVector3(clickPos);
            List<Thing> thingsAtCell = cell.GetThingList(pawn.Map);

            Thing relay = null;
            foreach (Thing thing in thingsAtCell)
            {
                if (thing.def?.defName == "Cerebrex_Relay")
                {
                    relay = thing;
                    break;
                }
            }

            if (relay == null)
            {
                return;
            }

            // Check if relay is powered
            CompPowerTrader powerComp = relay.TryGetComp<CompPowerTrader>();
            if (powerComp != null && !powerComp.PowerOn)
            {
                opts.Add(new FloatMenuOption("Call Archotech (no power)", null));
                return;
            }

            // Check if pawn has Cerebrex Node
            if (!HasCerebrexNode(pawn))
            {
                opts.Add(new FloatMenuOption("Call Archotech (requires Cerebrex Node)", null));
                return;
            }

            // Add interaction option
            opts.Add(new FloatMenuOption("Call Archotech", () => OpenArchotechDialog(pawn, relay)));
        }

        private static bool HasCerebrexNode(Pawn pawn)
        {
            if (pawn?.apparel?.WornApparel == null)
            {
                return false;
            }

            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                if (apparel.def?.defName == "Apparel_CerebrexNode")
                {
                    return true;
                }
            }

            return false;
        }

        private static void OpenArchotechDialog(Pawn pawn, Thing relay)
        {
            // Reveal faction if first contact
            RevealArchotechFaction();

            // Get faction
            Faction archotechFaction = Find.FactionManager.FirstFactionOfDef(CerebrexDefOf.Cerebrex_ArchotechFaction);

            if (archotechFaction == null)
            {
                Log.Error("[CerebrexRebalance] Archotech faction not found!");
                Messages.Message("Unable to establish connection.", MessageTypeDefOf.RejectInput);
                return;
            }

            // Open faction dialog
            Find.WindowStack.Add(new Dialog_Negotiate(archotechFaction, pawn));
        }

        private static void RevealArchotechFaction()
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
                // Set faction as not hidden (mark as discovered)
                archotechRevealed = true;

                // Send notification
                Find.LetterStack.ReceiveLetter(
                    "First Contact",
                    "You have established contact with the Archotechs. An ancient intelligence now acknowledges your existence.",
                    LetterDefOf.PositiveEvent
                );

                Log.Message("[CerebrexRebalance] Archotech faction revealed");
            }
        }

        /// <summary>
        /// Reset state for new game.
        /// </summary>
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
    /// Reset relay state on new game.
    /// </summary>
    [HarmonyPatch(typeof(Game), "InitNewGame")]
    public static class Game_InitNewGame_RelayClear_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            RelayInteraction_Patch.ResetState();
        }
    }
}
