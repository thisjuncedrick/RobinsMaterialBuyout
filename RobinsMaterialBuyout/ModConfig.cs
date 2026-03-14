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
    }

    private void ResetConfig(IModHelper helper)
    {
      UseMod = true;
      ShowInfoBoard = true;
      UseBasePrice = false;

      helper.WriteConfig(this);
    }
  }
}
