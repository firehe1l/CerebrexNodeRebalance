using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CerebrexRebalance
{
    /// <summary>
    /// SitePartWorker that ensures the Signal Amplifier spawns in the map.
    /// Replaces unreliable GenStep injection.
    /// </summary>
    public class SitePartWorker_SpawnAmplifier : SitePartWorker
    {
        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);
            SpawnSignalAmplifier(map);
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
                Log.Message("[CerebrexRebalance] Signal Amplifier spawned at " + spawnCell);
            }
            else
            {
                Log.Warning("[CerebrexRebalance] Could not find valid spawn cell for Signal Amplifier");
            }
        }

        private IntVec3 FindSpawnCell(Map map)
        {
            // NEW LOGIC: Iterating all valid rooms to find the complex
            HashSet<Room> validRooms = new HashSet<Room>();

            foreach (Region region in map.regionGrid.AllRegions_NoRebuild_InvalidAllowed)
            {
                Room room = region.Room;
                if (room != null && !room.PsychologicallyOutdoors && !room.IsDoorway)
                {
                    // Check if room has roof
                    bool hasRoof = false;
                    foreach (IntVec3 cell in room.Cells)
                    {
                        if (cell.Roofed(map))
                        {
                            hasRoof = true;
                            break;
                        }
                    }

                    if (hasRoof)
                    {
                        validRooms.Add(room);
                    }
                }
            }

            if (validRooms.Count > 0)
            {
                // Sort by size descending (usually main complex rooms are larger)
                List<Room> sortedRooms = validRooms.OrderByDescending(r => r.CellCount).ToList();

                // Try top 3 largest rooms
                // This targets the main structure rather than small side closets
                int candidates = System.Math.Min(sortedRooms.Count, 3);
                Room selectedRoom = sortedRooms.Take(candidates).RandomElement();
                
                // Try to find a walkable cell in this room
                if (selectedRoom != null)
                {
                   foreach (IntVec3 cell in selectedRoom.Cells) 
                   {
                       if (cell.Walkable(map) && cell.GetEdifice(map) == null) // Empty floor
                       {
                           return cell;
                       }
                   }
                   // Fallback: any cell in room
                   return selectedRoom.Cells.RandomElement(); 
                }
            }

            // Fallback: any valid roofed walkable cell near center
            IntVec3 mapCenter = map.Center;
            for (int i = 0; i < 100; i++)
            {
                 IntVec3 cell = CellFinder.RandomClosewalkCellNear(mapCenter, map, 20);
                 if (cell.IsValid && cell.Walkable(map) && cell.Roofed(map))
                 {
                     return cell;
                 }
            }

            // Last resort: random cell
            return CellFinder.RandomCell(map);
        }
    }
}
