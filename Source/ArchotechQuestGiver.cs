using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CerebrexRebalance
{
    /// <summary>
    /// Triggers quest and creates world site when Cerebrex Node is first equipped.
    /// Creates an ancient complex site directly instead of relying on QuestScriptDef.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Wear")]
    public static class ArchotechQuestGiver_Patch
    {
        // Track if quest has been given this playthrough
        private static bool questGiven = false;

        [HarmonyPostfix]
        public static void Postfix(Apparel newApparel, Pawn ___pawn)
        {
            if (newApparel == null || ___pawn == null)
            {
                return;
            }

            // Check if it's Cerebrex Node
            if (newApparel.def != null && newApparel.def.defName != "Apparel_CerebrexNode")
            {
                return;
            }

            // Only trigger once per game
            if (questGiven)
            {
                return;
            }

            // Check if site/quest already exists
            if (HasArchotechSite())
            {
                questGiven = true;
                return;
            }

            // Generate the site directly
            TryGenerateArchotechSite(___pawn);
        }

        private static bool HasArchotechSite()
        {
            if (Find.WorldObjects == null || Find.WorldObjects.Sites == null)
            {
                return false;
            }

            foreach (Site site in Find.WorldObjects.Sites)
            {
                if (site == null || site.parts == null) continue;
                
                foreach (SitePart part in site.parts)
                {
                    // Check if this is our signal amplifier site
                    if (part.def != null && part.def.defName == "Cerebrex_SignalAmplifierSite")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void TryGenerateArchotechSite(Pawn pawn)
        {
            try
            {
                Map map = (pawn != null ? pawn.Map : null) ?? Find.CurrentMap;
                if (map == null)
                {
                    Log.Warning("[CerebrexRebalance] No map found, cannot generate site");
                    return;
                }

                // Find a valid tile for the site
                int destinationTile = FindDestinationTile(map.Tile);
                if (destinationTile == -1)
                {
                    Log.Warning("[CerebrexRebalance] Could not find valid tile for archotech site");
                    return;
                }

                // Get threat points for site generation
                float points = StorytellerUtility.DefaultThreatPointsNow(map);

                // Create site with Ancient Complex
                Site site = CreateArchotechSite(destinationTile, points);
                if (site == null)
                {
                    Log.Warning("[CerebrexRebalance] Failed to create site");
                    return;
                }

                // Add to world
                Find.WorldObjects.Add(site);
                questGiven = true;

                // Notify player with coordinates
                string coordinates = Find.WorldGrid.LongLatOf(destinationTile).ToString();
                Find.LetterStack.ReceiveLetter(
                    "Archotech Signal Detected",
                    string.Format("Equipping the Cerebrex Node has awakened dormant systems. Coordinates for an ancient mechanoid relay station have been transmitted to your colony.\n\nThe site is marked on your world map at approximately {0}.\n\nIntelligence suggests a Signal Amplifier may be found within - essential for constructing an Archotech Relay.\n\nWarning: The site is defended by hostile mechanoids.", coordinates),
                    LetterDefOf.PositiveEvent,
                    new LookTargets(site)
                );

                Log.Message("[CerebrexRebalance] Archotech site generated at tile " + destinationTile);
            }
            catch (System.Exception ex)
            {
                Log.Error("[CerebrexRebalance] Failed to generate archotech site: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private static int FindDestinationTile(int sourceTile)
        {
            // Try to find a tile 3-10 days of travel away
            int minDist = 3;
            int maxDist = 10;

            for (int attempt = 0; attempt < 100; attempt++)
            {
                int tile = TileFinder.RandomSettlementTileFor(null, false, 
                    t => IsValidSiteTile(t, sourceTile, minDist, maxDist));
                
                if (tile != -1)
                {
                    return tile;
                }
            }

            /* Fallback disabled due to API mismatch in 1.6
            List<int> candidates = new List<int>();
            WorldFloodFiller floodFiller = Traverse.Create(Find.World).Property("FloodFiller").GetValue<WorldFloodFiller>();
            if (floodFiller == null) floodFiller = Traverse.Create(Find.World).Field("floodFiller").GetValue<WorldFloodFiller>();

            if (floodFiller != null)
            {
                floodFiller.FloodFill(sourceTile, 
                    t => !Find.World.Impassable(t),
                    delegate(int t)
                    {
                         // Logic removed
                    }, 
                    5000
                );
            }

            if (candidates.Count > 0)
            {
                return candidates.RandomElement();
            }
            */

            return -1;
        }

        private static bool IsValidSiteTile(int tile, int sourceTile, int minDays, int maxDays)
        {
            if (tile == sourceTile)
            {
                return false;
            }

            if (!TileFinder.IsValidTileForNewSettlement(tile))
            {
                return false;
            }

            // Check for existing world objects
            if (Find.WorldObjects.AnyWorldObjectAt(tile))
            {
                return false;
            }

            // Check distance
            if (Find.WorldGrid.ApproxDistanceInTiles(sourceTile, tile) < minDays * 5)
            {
                return false;
            }

            return true;
        }

        private static Site CreateArchotechSite(int tile, float points)
        {
            // Get Ancient Complex site part def
            SitePartDef ancientComplexDef = DefDatabase<SitePartDef>.GetNamedSilentFail("AncientComplex");
            if (ancientComplexDef == null)
            {
                Log.Error("[CerebrexRebalance] AncientComplex SitePartDef not found!");
                return null;
            }

            // Get our custom site part for Signal Amplifier
            SitePartDef amplifierSiteDef = DefDatabase<SitePartDef>.GetNamedSilentFail("Cerebrex_SignalAmplifierSite");

            // Create site parts
            SitePartParams partParams = new SitePartParams
            {
                threatPoints = System.Math.Max(points, 500f)
            };

            List<SitePartDefWithParams> parts = new List<SitePartDefWithParams>
            {
                new SitePartDefWithParams(ancientComplexDef, partParams)
            };

            // Add our custom part if it exists
            if (amplifierSiteDef != null)
            {
                parts.Add(new SitePartDefWithParams(amplifierSiteDef, partParams));
            }

            // Generate the site
            Site site = SiteMaker.MakeSite(parts, tile, null, false);
            
            if (site != null)
            {
                // Set custom label
                site.customLabel = "Archotech Relay Station";
                
                // Set timeout (60 days)
                if (site.GetComponent<TimeoutComp>() != null)
                {
                    site.GetComponent<TimeoutComp>().StartTimeout(60 * 60000);
                }
            }

            return site;
        }

        /// <summary>
        /// Reset quest state for new game.
        /// </summary>
        public static void ResetQuestState()
        {
            questGiven = false;
        }
    }

    /// <summary>
    /// Reset quest state when starting new game.
    /// </summary>
    [HarmonyPatch(typeof(Game), "InitNewGame")]
    public static class Game_InitNewGame_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ArchotechQuestGiver_Patch.ResetQuestState();
        }
    }

    /// <summary>
    /// Reset quest state when loading game (check for existing site).
    /// </summary>
    [HarmonyPatch(typeof(Game), "LoadGame")]
    public static class Game_LoadGame_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ArchotechQuestGiver_Patch.ResetQuestState();
        }
    }
}
