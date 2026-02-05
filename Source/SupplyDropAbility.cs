using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CerebrexRebalance
{
    public class CompProperties_AbilitySupplyDrop : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySupplyDrop()
        {
            this.compClass = typeof(CompAbilityEffect_SupplyDrop);
        }
    }

    public class CompAbilityEffect_SupplyDrop : CompAbilityEffect
    {
        // Cached target for menu selection
        private IntVec3 cachedTargetCell;
        private Map cachedMap;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (this.parent?.pawn?.Map == null)
            {
                return;
            }

            // Cache target location for menu callbacks
            cachedMap = this.parent.pawn.Map;
            cachedTargetCell = target.Cell;

            // Show resource selection menu
            List<FloatMenuOption> options = new List<FloatMenuOption>
            {
                new FloatMenuOption("Steel (350)", () => SpawnSupplyDrop(ThingDefOf.Steel, 350)),
                new FloatMenuOption("Plasteel (100)", () => SpawnSupplyDrop(ThingDefOf.Plasteel, 100)),
                new FloatMenuOption("Components (20)", () => SpawnSupplyDrop(ThingDefOf.ComponentIndustrial, 20)),
                new FloatMenuOption("Adv. Components (5)", () => SpawnSupplyDrop(ThingDefOf.ComponentSpacer, 5)),
                new FloatMenuOption("Medicine (25)", () => SpawnSupplyDrop(ThingDefOf.MedicineIndustrial, 25))
            };

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private void SpawnSupplyDrop(ThingDef thingDef, int count)
        {
            if (cachedMap == null || !cachedTargetCell.IsValid)
            {
                Log.Warning("[CerebrexRebalance] Invalid target for supply drop");
                return;
            }

            try
            {
                Thing thing = ThingMaker.MakeThing(thingDef);
                thing.stackCount = count;

                // Drop at targeted location
                DropPodUtility.DropThingsNear(
                    cachedTargetCell,
                    cachedMap,
                    new List<Thing> { thing },
                    forbid: false,
                    canRoofPunch: true
                );

                Messages.Message(
                    "Mechanoid supply drop arrived.",
                    new TargetInfo(cachedTargetCell, cachedMap),
                    MessageTypeDefOf.PositiveEvent
                );
            }
            catch (Exception ex)
            {
                Log.Error($"[CerebrexRebalance] Error spawning supply drop: {ex.Message}");
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!base.CanApplyOn(target, dest))
            {
                return false;
            }

            if (this.parent?.pawn?.Map == null)
            {
                return false;
            }

            // Check if location is valid for dropping
            IntVec3 cell = target.Cell;
            Map map = this.parent.pawn.Map;

            if (!cell.InBounds(map) || !cell.Walkable(map))
            {
                return false;
            }

            return true;
        }
    }
}

