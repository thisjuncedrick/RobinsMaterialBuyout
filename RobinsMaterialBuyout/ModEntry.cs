using HarmonyLib;

using RobinsMaterialBuyout.Patches;
using RobinsMaterialBuyout.Services;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

namespace RobinsMaterialBuyout
{
  public class ModEntry : Mod
  {
#if DEBUG
    private const LogLevel DefaultLogLevel = LogLevel.Debug;
#else
    private const LogLevel DefaultLogLevel = LogLevel.Trace;
#endif

    private ModConfig _config = null!;
    private static IMonitor? _monitor;

    public override void Entry(IModHelper helper)
    {
      _monitor = Monitor;
      I18n.Init(helper.Translation);

      _config = helper.ReadConfig<ModConfig>();

      var harmony = new Harmony(ModManifest.UniqueID);
      
      CarpenterMenuPatch.Initialize(helper, _config);
      CarpenterMenuPatch.ApplyPatches(harmony);

      helper.Events.GameLoop.GameLaunched += (_, _) => _config.RegisterGMCM(Helper, ModManifest);
      // Invalidates cached prices. Preventing stale prices when profession affects material pricing (e.g., Artisan, Blacksmith)
      helper.Events.Player.LevelChanged += (_, _) => MaterialCostService.ClearCache();
      helper.Events.Input.ButtonPressed   += OnButtonPressed;
    }
      
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
      if (!Context.IsPlayerFree)
        return;

#if DEBUG
      switch (e.Button)
      {
        case SButton.M:
          Game1.activeClickableMenu = new CarpenterMenu(Game1.builder_robin);
          break;

        case SButton.N:
          Game1.activeClickableMenu = new CarpenterMenu(Game1.builder_wizard);
          break;
      }
#endif
    }

    internal static void Log(string message, LogLevel level = DefaultLogLevel) =>
      _monitor?.Log(message, level);
  }
}