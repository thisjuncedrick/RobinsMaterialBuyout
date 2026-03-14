using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RobinsMaterialBuyout.Models;

using StardewValley;
using StardewValley.Menus;

namespace RobinsMaterialBuyout.UI
{
  internal class InfoBoardRenderer
  {
    private static readonly Rectangle CoinSource = new(241, 303, 14, 13);

    public static void Draw(SpriteBatch b, CarpenterMenu menu, BuyoutModel model)
    {
      var theme = new UITheme(menu.Blueprint.MagicalConstruction);
      var (x, y) = CalculateAnchor(menu);

      IClickableMenu.drawTextureBox(b, x, y, model.Width, model.Height, theme.BoxColor);

      int currentY = y + BoardUIConstants.Padding;
      int textX = x + BoardUIConstants.Padding;

      // Header
      Utility.drawTextWithShadow(b, I18n.UI_InfoBoard_Header(), Game1.smallFont, new Vector2(textX, currentY), theme.TextColor, shadowIntensity: theme.ShadowAlpha);
      currentY += (int)Game1.smallFont.MeasureString(I18n.UI_InfoBoard_Header()).Y + BoardUIConstants.Spacing;

      // Materials
      foreach (var m in model.MissingMaterials)
      {
        var data = ItemRegistry.GetDataOrErrorItem(m.Item.QualifiedItemId);
        b.Draw(data.GetTexture(), new Vector2(textX, currentY), data.GetSourceRect(), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);

        Utility.drawTextWithShadow(b, $"{m.Need}x", Game1.smallFont, new Vector2(textX + 42, currentY + 4), theme.TextColor, shadowIntensity: theme.ShadowAlpha);

        string cost = $"+{Utility.getNumberWithCommas(m.Cost)}g";
        float costW = Game1.smallFont.MeasureString(cost).X;
        Utility.drawTextWithShadow(b, cost, Game1.smallFont, new Vector2(x + model.Width - BoardUIConstants.Padding - costW, currentY + 4), theme.CostColor, shadowIntensity: theme.ShadowAlpha);

        currentY += Math.Max(BoardUIConstants.MaterialIconSize, Game1.smallFont.LineSpacing);
      }

      DrawDivider(b, textX, ref currentY, model.Width, theme.DividerColor);

      // Sub-totals
      DrawRow(b, I18n.UI_InfoBoard_Total_MaterialCost(), $"{Utility.getNumberWithCommas(model.MaterialCost)}g", textX, ref currentY, model.Width, theme);
      DrawRow(b, I18n.UI_InfoBoard_Total_BuildPrice(), $"+{Utility.getNumberWithCommas(menu.Blueprint.BuildCost)}g", textX, ref currentY, model.Width, theme);

      DrawDivider(b, textX, ref currentY, model.Width, theme.DividerColor);

      // Total
      Color finalColor = Game1.player.Money < model.TotalCost ? Color.Red : theme.CostColor;
      DrawRow(b, I18n.UI_InfoBoard_Total_TotalPayment(), $"{Utility.getNumberWithCommas(model.TotalCost)}g", textX, ref currentY, model.Width, theme, finalColor);
    }

    private static void DrawRow(SpriteBatch b, string label, string val, int x, ref int y, int w, UITheme theme, Color? overrideValColor = null)
    {
      b.Draw(Game1.mouseCursors_1_6, new Vector2(x, y), CoinSource, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
      Utility.drawTextWithShadow(b, label, Game1.smallFont, new Vector2(x + 38, y + 2), theme.TextColor, shadowIntensity: theme.ShadowAlpha);

      float valW = Game1.smallFont.MeasureString(val).X;
      Utility.drawTextWithShadow(b, val, Game1.smallFont, new Vector2(x + w - (BoardUIConstants.Padding * 2) - valW, y + 2), overrideValColor ?? theme.CostColor, shadowIntensity: theme.ShadowAlpha);
      y += Game1.smallFont.LineSpacing;
    }

    private static void DrawDivider(SpriteBatch b, int x, ref int y, int w, Color color)
    {
      y += BoardUIConstants.Spacing;
      b.Draw(Game1.staminaRect, new Rectangle(x, y, w - (BoardUIConstants.TotalPadding), 2), color);
      y += BoardUIConstants.Spacing;
    }

    private static (int x, int y) CalculateAnchor(CarpenterMenu menu)
    {
      int x = menu.xPositionOnScreen - 96 + menu.width + 64 - 8;
      int y = menu.yPositionOnScreen + 80 + (menu.Blueprint.MagicalConstruction ? 0 : 80);
      return (x, y);
    }
  }
}
