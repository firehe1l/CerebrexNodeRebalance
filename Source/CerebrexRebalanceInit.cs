using HarmonyLib;
using Verse;

namespace CerebrexRebalance
{
    [StaticConstructorOnStartup]
    public static class CerebrexRebalanceInit
    {
        static CerebrexRebalanceInit()
        {
            var harmony = new Harmony("Antigravity.CerebrexRebalance");
            harmony.PatchAll();
            Log.Message("[CerebrexRebalance] Loaded successfully");
        }
    }
}
