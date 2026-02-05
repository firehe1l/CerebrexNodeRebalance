using RimWorld;
using Verse;

namespace CerebrexRebalance
{
    /// <summary>
    /// DefOf class for all Cerebrex mod definitions.
    /// Uses caching via DefOf pattern for performance.
    /// </summary>
    [DefOf]
    public static class CerebrexDefOf
    {
        // Faction
        public static FactionDef Cerebrex_ArchotechFaction;

        // Items
        public static ThingDef Cerebrex_SignalAmplifier;

        // Buildings
        public static ThingDef Cerebrex_Relay;

        // Abilities
        public static AbilityDef Cerebrex_SupplyDrop;
        public static AbilityDef Cerebrex_OrbitalStrike;

        // Jobs
        public static JobDef Cerebrex_UseRelay;

        // Apparel (vanilla Odyssey)
        [MayRequire("Ludeon.RimWorld.Odyssey")]
        public static ThingDef Apparel_CerebrexNode;

        static CerebrexDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(CerebrexDefOf));
        }
    }
}
