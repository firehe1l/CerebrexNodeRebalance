using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace CerebrexRebalance
{
    /// <summary>
    /// Properties for the relay usable component.
    /// </summary>
    public class CompProperties_RelayUsable : CompProperties
    {
        public string useLabel = "Contact Archotechs";
        public int useDuration = 120;

        public CompProperties_RelayUsable()
        {
            compClass = typeof(CompRelayUsable);
        }
    }

    /// <summary>
    /// Component that allows pawns with Cerebrex Node to use the Archotech Relay.
    /// </summary>
    public class CompRelayUsable : ThingComp
    {
        public CompProperties_RelayUsable Props
        {
            get
            {
                return (CompProperties_RelayUsable)props;
            }
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (selPawn == null)
            {
                yield break;
            }

            // Check if pawn can reach the interaction cell
            if (!selPawn.CanReach(parent, PathEndMode.InteractionCell, Danger.Deadly))
            {
                yield return new FloatMenuOption(Props.useLabel + " (cannot reach)", null);
                yield break;
            }

            // Check if pawn is wearing Cerebrex Node
            if (!HasCerebrexNode(selPawn))
            {
                yield return new FloatMenuOption(Props.useLabel + " (requires Cerebrex Node)", null);
                yield break;
            }

            // Check power
            CompPowerTrader power = parent.TryGetComp<CompPowerTrader>();
            if (power != null && !power.PowerOn)
            {
                yield return new FloatMenuOption(Props.useLabel + " (no power)", null);
                yield break;
            }

            // Valid use option
            yield return new FloatMenuOption(Props.useLabel, delegate
            {
                Job job = JobMaker.MakeJob(CerebrexDefOf.Cerebrex_UseRelay, parent);
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });
        }

        private bool HasCerebrexNode(Pawn pawn)
        {
            if (pawn == null || pawn.apparel == null || pawn.apparel.WornApparel == null)
            {
                return false;
            }

            foreach (Apparel apparel in pawn.apparel.WornApparel)
            {
                if (apparel != null && apparel.def != null && apparel.def.defName == "Apparel_CerebrexNode")
                {
                    return true;
                }
            }

            return false;
        }

        public void UseRelay(Pawn user)
        {
            if (user == null)
            {
                return;
            }

            // Reveal archotech faction and show dialog
            ArchotechFactionManager.RevealArchotechFaction();

            // Get faction and show dialog
            Faction archotechFaction = Find.FactionManager.FirstFactionOfDef(CerebrexDefOf.Cerebrex_ArchotechFaction);
            if (archotechFaction != null)
            {
                Find.WindowStack.Add(new Dialog_Negotiate(archotechFaction, user));
            }
            else
            {
                Log.Warning("[CerebrexRebalance] Archotech faction not found when using relay");
                Messages.Message("No response from Archotech intelligences.", MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
