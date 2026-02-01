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
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            List<FloatMenuOption> options = new List<FloatMenuOption>();

            options.Add(new FloatMenuOption("Steel (350)", new Action(this.DropSteel)));
            options.Add(new FloatMenuOption("Plasteel (100)", new Action(this.DropPlasteel)));
            options.Add(new FloatMenuOption("Components (20)", new Action(this.DropComponents)));
            options.Add(new FloatMenuOption("Adv. Components (5)", new Action(this.DropAdvComponents)));
            options.Add(new FloatMenuOption("Medicine (25)", new Action(this.DropMedicine)));

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private void DropSteel()
        {
            this.SpawnSupplyDrop(ThingDefOf.Steel, 350);
        }

        private void DropPlasteel()
        {
            this.SpawnSupplyDrop(ThingDefOf.Plasteel, 100);
        }

        private void DropComponents()
        {
            this.SpawnSupplyDrop(ThingDefOf.ComponentIndustrial, 20);
        }

        private void DropAdvComponents()
        {
            this.SpawnSupplyDrop(ThingDefOf.ComponentSpacer, 5);
        }

        private void DropMedicine()
        {
            this.SpawnSupplyDrop(ThingDefOf.MedicineIndustrial, 25);
        }

        private void SpawnSupplyDrop(ThingDef thingDef, int count)
        {
            Map map = this.parent.pawn.Map;
            IntVec3 cell = DropCellFinder.TradeDropSpot(map);

            Thing t = ThingMaker.MakeThing(thingDef);
            t.stackCount = count;

            // Use DropPodUtility for RimWorld 1.6
            DropPodUtility.DropThingsNear(cell, map, new List<Thing> { t }, forbid: false, canRoofPunch: true);
            Messages.Message("Mechanoid supply drop arrived.", new TargetInfo(cell, map), MessageTypeDefOf.PositiveEvent);
        }
    }
}
