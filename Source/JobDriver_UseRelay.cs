using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CerebrexRebalance
{
    /// <summary>
    /// Job driver for using the Archotech Relay.
    /// </summary>
    public class JobDriver_UseRelay : JobDriver
    {
        private const int UseDuration = 120;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Fail conditions
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOn(() => !HasCerebrexNode(pawn));

            // CompPower check
            CompPowerTrader power = TargetThingA?.TryGetComp<CompPowerTrader>();
                if (power != null)
                {
                    this.FailOn(() => !power.PowerOn);
                }

            // Go to interaction cell
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            // Use the relay
            Toil useToil = ToilMaker.MakeToil("UseRelay");
            useToil.initAction = delegate
            {
                pawn.rotationTracker.FaceCell(TargetThingA.Position);
            };
            useToil.tickAction = delegate
            {
                if (pawn.skills != null)
                {
                    pawn.skills.Learn(SkillDefOf.Intellectual, 0.035f);
                }
                pawn.rotationTracker.FaceCell(TargetThingA.Position);
            };
            useToil.defaultCompleteMode = ToilCompleteMode.Delay;
            useToil.defaultDuration = UseDuration;
            useToil.WithProgressBarToilDelay(TargetIndex.A);
            yield return useToil;

            // Complete - trigger relay effect
            Toil finishToil = ToilMaker.MakeToil("FinishUseRelay");
            finishToil.initAction = delegate
            {
                CompRelayUsable comp = null;
                if (TargetThingA != null)
                {
                    comp = TargetThingA.TryGetComp<CompRelayUsable>();
                }
                if (comp != null)
                {
                    comp.UseRelay(pawn);
                }
            };
            finishToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return finishToil;
        }

        private bool HasCerebrexNode(Pawn p)
        {
            if (p == null || p.apparel == null || p.apparel.WornApparel == null)
            {
                return false;
            }

            foreach (Apparel apparel in p.apparel.WornApparel)
            {
                if (apparel != null && apparel.def != null && apparel.def.defName == "Apparel_CerebrexNode")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
