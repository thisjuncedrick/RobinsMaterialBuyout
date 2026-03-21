using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

using RobinsMaterialBuyout.Models;
using RobinsMaterialBuyout.Services;
using RobinsMaterialBuyout.UI;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace RobinsMaterialBuyout.Patches
{
  internal class CarpenterMenuPatch
  {
    private static IModHelper _helper = null!;
    private static ModConfig _config = null!;
    private static BuyoutModel? _cachedModel;
    private static string? _tooltipText;
    private static string? _lastBlueprintKey;
    private static bool _isInventoryDirty = true;

    public static void Initialize(IModHelper helper, ModConfig config)
    {
      _helper = helper;
      _config = config;
    }

    public static void ApplyPatches(Harmony harmony)
    {
      harmony.Patch(
        original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new[] { typeof(SpriteBatch) }),
        postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(Draw_Postfix))
      );

      harmony.Patch(
        original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.CanBuildCurrentBlueprint)),
        postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(CanBuild_Postfix))
      );

      harmony.Patch(
        original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.ConsumeResources)),
        prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(Consume_Prefix))
      );

      harmony.Patch(
        original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.performHoverAction)),
        postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(PerformHoverAction_Postfix))
      );

      _helper.Events.Player.InventoryChanged += OnInventoryChanged;
      _helper.Events.Display.MenuChanged += OnMenuChanged;
    }

    private static void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
      if (e.NewMenu is CarpenterMenu)
      {
        _lastBlueprintKey = null;
        _isInventoryDirty = true;
        _cachedModel = null;
        _tooltipText = null;
      }
    }

    private static void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
      if (!_config.UseMod || !e.Player.IsLocalPlayer || Game1.activeClickableMenu is not CarpenterMenu)
        return;

      _isInventoryDirty = true;
    }
      
    private static void Draw_Postfix(CarpenterMenu __instance, SpriteBatch b)
    {
      if (!IsMenuActive(__instance)) 
        return;

      UpdateCache(__instance);

      if (_cachedModel != null && _config.ShowInfoBoard)
        InfoBoardRenderer.Draw(b, __instance, _cachedModel);

      __instance.drawMouse(b);
    }

    private static void CanBuild_Postfix(CarpenterMenu __instance, ref bool __result)
    {
      if (!IsMenuActive(__instance) || __result || _cachedModel == null) 
        return;

      __result = Game1.player.Money >= _cachedModel.TotalCost;
    }

    private static void Consume_Prefix(CarpenterMenu __instance)
    {
      if (!_config.UseMod || _cachedModel == null) 
        return;

      Game1.player.Money -= _cachedModel.MaterialCost;
      ModEntry.Log($"Purchased missing materials for {__instance.Blueprint.Id}: {_cachedModel.MaterialCost:N0}g");
    }

    private static void PerformHoverAction_Postfix(CarpenterMenu __instance, int x, int y)
    {
      if (!IsMenuActive(__instance) || _config.ShowInfoBoard || _tooltipText == null) return;

      if (__instance.okButton.containsPoint(x, y))
        _helper.Reflection.GetField<string>(__instance, "hoverText").SetValue(_tooltipText);
    }
    
    private static void UpdateCache(CarpenterMenu menu)
    {
      string currentBlueprintKey = menu.Blueprint.Id;
      bool blueprintChanged = _lastBlueprintKey != currentBlueprintKey;

      // Nothing changed, use the cache so exit early
      if (!blueprintChanged && !_isInventoryDirty)
        return;

      if (blueprintChanged)
        ModEntry.Log($"Blueprint changed: {_lastBlueprintKey ?? "None"} -> {currentBlueprintKey}");
      else if (_isInventoryDirty)
        ModEntry.Log($"Inventory changed.");
      
      _lastBlueprintKey = currentBlueprintKey;

      //Rebuild data model
      var missing = MaterialCostService.GetMissingMaterials(menu.ingredients, _config.UseBasePrice, _config.PriceIncreaseRate);

      if (missing.Any())
      {
        _cachedModel = new BuyoutModel(missing, menu.Blueprint.BuildCost);
        ModEntry.Log($"Model Refreshed: Missing {missing.Count} material types. Total Buy-out: {_cachedModel.MaterialCost:N0}g");

        // Update tooltip text if board is not rendered
        if (!_config.ShowInfoBoard)
        {
          _tooltipText = BuildTooltipText(menu.Blueprint, _cachedModel);

          // Push tooltip change if the mouse happens to be there right now
          if (menu.okButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            _helper.Reflection.GetField<string>(menu, "hoverText").SetValue(_tooltipText);
        }
      }
      else
      {
        _cachedModel = null;
        _tooltipText = null;
        ModEntry.Log("No materials missing. Cache cleared.");
      }

      _isInventoryDirty = false;
    }

    private static string BuildTooltipText(CarpenterMenu.BlueprintEntry bp, BuyoutModel model)
    {
      return $"{I18n.UI_InfoBoard_Total_MaterialCost()}: {Utility.getNumberWithCommas(model.MaterialCost)}g\n" +
             $"{I18n.UI_InfoBoard_Total_BuildPrice()}: +{Utility.getNumberWithCommas(bp.BuildCost)}g\n" +
             $"{I18n.UI_InfoBoard_Total_TotalPayment()}: {Utility.getNumberWithCommas(model.TotalCost)}g";
    }

    private static bool IsMenuActive(CarpenterMenu menu) =>
      _config.UseMod && menu != null && !menu.freeze && !menu.onFarm && !Game1.IsFading();
  }
}
