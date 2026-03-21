using RobinsMaterialBuyout.Integrations.GenericModConfigMenu;
using RobinsMaterialBuyout.Services;
using StardewModdingAPI;

namespace RobinsMaterialBuyout
{
  internal class ModConfig
  {
    public bool UseMod { get; set; } = true;
    public bool ShowInfoBoard { get; set; } = true;
    public bool UseBasePrice { get; set; } = false;
    public float PriceIncreaseRate { get; set; } = 0.5f;

    public void RegisterGMCM(IModHelper helper, IManifest manifest)
    {
      var gmcm = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

      if (gmcm is null)
        return;

      gmcm.Register(
        mod:  manifest,
        reset:() => ResetConfig(helper),
        save: () => {
          MaterialCostService.ClearCache();
          helper.WriteConfig(this);
        }
      );

      gmcm.AddBoolOption(
        mod:      manifest,
        name:     () => I18n.Config_UseMod_Name(),
        tooltip:  () => I18n.Config_UseMod_Tooltip(),
        getValue: () => UseMod,
        setValue: v => UseMod = v
      );

      gmcm.AddBoolOption(
        mod:      manifest,
        name:     () => I18n.Config_ShowInfo_Name(),
        tooltip:  () => I18n.Config_ShowInfo_Tooltip(),
        getValue: () => ShowInfoBoard,
        setValue: v => ShowInfoBoard = v
      );

      gmcm.AddBoolOption(
        mod:      manifest,
        name:     () => I18n.Config_UseBasePrice_Name(),
        tooltip:  () => I18n.Config_UseBasePrice_Tooltip(),
        getValue: () => UseBasePrice,
        setValue: v => UseBasePrice = v
      );

      gmcm.AddNumberOption(
        mod:      manifest,
        name:     () => I18n.Config_PriceIncreaseRate_Name(),
        tooltip:  () => I18n.Config_PriceIncreaseRate_Tooltip(),
        getValue: () => PriceIncreaseRate,
        setValue: v => PriceIncreaseRate = v

      );
    }

    private void ResetConfig(IModHelper helper)
    {
      UseMod = true;
      ShowInfoBoard = true;
      UseBasePrice = false;
      PriceIncreaseRate = 0.5f;

      helper.WriteConfig(this);
    }
  }
}
