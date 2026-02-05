using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CerebrexRebalance
{
    /// <summary>
    /// CompProperties for orbital strike ability.
    /// </summary>
    public class CompProperties_AbilityOrbitalStrike : CompProperties_AbilityEffect
    {
        public float explosionRadius = 4.9f;
        public int explosionCount = 8;
        public int durationTicks = 300;

        public CompProperties_AbilityOrbitalStrike()
        {
            this.compClass = typeof(CompAbilityEffect_OrbitalStrike);
        }
    }

    /// <summary>
    /// Orbital strike ability effect - spawns bombardment at target location.
    /// Similar to Imperial permit orbital bombardment.
    /// </summary>
    public class CompAbilityEffect_OrbitalStrike : CompAbilityEffect
    {
        private new CompProperties_AbilityOrbitalStrike Props => (CompProperties_AbilityOrbitalStrike)this.props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (!target.HasThing && !target.Cell.IsValid || this.parent?.pawn?.Map == null)
            {
                return;
            }

            Map map = this.parent.pawn.Map;
            IntVec3 targetCell = target.Cell;

            try
            {
                // Spawn bombardment effect at target
                SpawnBombardment(map, targetCell);
                
                Messages.Message(
                    "Orbital mechanoid strike inbound!",
                    new TargetInfo(targetCell, map),
                    MessageTypeDefOf.ThreatBig
                );
            }
            catch (Exception ex)
            {
                Log.Error($"[CerebrexRebalance] Error in orbital strike: {ex.Message}");
            }
        }

        private void SpawnBombardment(Map map, IntVec3 targetCell)
        {
            // Use vanilla Bombardment class
            Bombardment bombardment = (Bombardment)GenSpawn.Spawn(ThingDefOf.Bombardment, targetCell, map);
            
            if (bombardment != null)
            {
                bombardment.duration = Props.durationTicks;
                bombardment.instigator = this.parent.pawn;
                bombardment.weaponDef = null;
                bombardment.StartStrike();
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!base.CanApplyOn(target, dest))
            {
                return false;
            }

            // Check if target is valid map location
            if (this.parent?.pawn?.Map == null)
            {
                return false;
            }

            Map map = this.parent.pawn.Map;
            IntVec3 cell = target.Cell;

            // Cannot target under thick roof
            if (cell.Roofed(map) && map.roofGrid.RoofAt(cell)?.isThickRoof == true)
            {
                return false;
            }

            return true;
        }

        public override bool GizmoDisabled(out string reason)
        {
            Pawn pawn = parent.pawn;
            if (pawn == null)
            {
                reason = "No pawn";
                return true;
            }

            // The original snippet had a syntax error here.
            // GizmoDisabled typically checks general conditions, not target-specific ones.
            // Assuming the intent was to check if the pawn is on a map.
            if (pawn.Map == null)
            {
                reason = "Cannot use without a map";
                return true;
            }

            reason = null;
            return false;
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            if (this.parent?.pawn?.Map == null)
            {
                return null;
            }

            Map map = this.parent.pawn.Map;
            IntVec3 cell = target.Cell;

            if (cell.Roofed(map) && map.roofGrid.RoofAt(cell)?.isThickRoof == true)
            {
                return "Cannot target under thick roof";
            }

            return null;
        }
    }
}
