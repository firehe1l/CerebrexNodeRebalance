using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;

namespace CerebrexRebalance
{
    /// <summary>
    /// GenStep that spawns Signal Amplifier in ancient complexes.
    /// Only spawns if the map is from the Archotech quest.
    /// </summary>
    public class GenStep_SpawnSignalAmplifier : GenStep
    {
        public override int SeedPart => 892374561;

        public override void Generate(Map map, GenStepParams parms)
        {
            // Only spawn in quest-related maps
            if (map?.Parent == null)
            {
                return;
            }

            // Check if this is our quest site (simplified check)
            // In production, use quest.linkedSite comparison
            if (!ShouldSpawnHere(map))
            {
                return;
            }

            SpawnSignalAmplifier(map);
        }

        private bool ShouldSpawnHere(Map map)
        {
            // Check if map parent is a Site with our quest
            Site site = map.Parent as Site;
            if (site == null)
            {
                return false;
            }

            // Check for ancient complex
            foreach (SitePart part in site.parts)
            {
                if (part.def?.defName == "AncientComplex")
                {
                    return true;
                }
            }

            return false;
        }

        private void SpawnSignalAmplifier(Map map)
        {
            ThingDef amplifierDef = CerebrexDefOf.Cerebrex_SignalAmplifier;
            if (amplifierDef == null)
            {
                Log.Error("[CerebrexRebalance] Signal Amplifier def not found!");
                return;
            }

            // Find a good spawn location (center area, indoor preferred)
            IntVec3 spawnCell = FindSpawnCell(map);

            if (spawnCell.IsValid)
            {
                Thing amplifier = ThingMaker.MakeThing(amplifierDef);
                GenPlace.TryPlaceThing(amplifier, spawnCell, map, ThingPlaceMode.Near);
                Log.Message($"[CerebrexRebalance] Signal Amplifier spawned at {spawnCell}");
            }
            else
            {
                Log.Warning("[CerebrexRebalance] Could not find valid spawn cell for Signal Amplifier");
            }
        }

        private IntVec3 FindSpawnCell(Map map)
        {
            // Try to find cell near map center
            IntVec3 mapCenter = map.Center;
            
            // Look for indoor cells first
            for (int i = 0; i < 100; i++)
            {
                IntVec3 cell = CellFinder.RandomClosewalkCellNear(mapCenter, map, 20);
                if (cell.IsValid && cell.Walkable(map) && cell.Roofed(map))
                {
                    return cell;
                }
            }

            // Fallback: any walkable cell near center
            for (int i = 0; i < 50; i++)
            {
                IntVec3 cell = CellFinder.RandomClosewalkCellNear(mapCenter, map, 30);
                if (cell.IsValid && cell.Walkable(map))
                {
                    return cell;
                }
            }

            // Last resort: random cell
            return CellFinder.RandomCell(map);
        }
    }
}
