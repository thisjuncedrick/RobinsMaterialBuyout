using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RobinsMaterialBuyout.Models;
using RobinsMaterialBuyout.UI;

using StardewValley;

namespace RobinsMaterialBuyout.Utilities
{
  internal static class LayoutMeasurer
  {
    public static (int Width, int Height) Measure(List<BuyoutMaterial> missing, int materialCost, int buildCost)
    {
      SpriteFont font = Game1.smallFont;
      Vector2 headerDim = font.MeasureString(I18n.UI_InfoBoard_Header());
      float maxW = headerDim.X;

      foreach (var m in missing)
      {
        float rowW = BoardUIConstants.MaterialIconSize + BoardUIConstants.Spacing +
                     font.MeasureString($"{m.Need}x").X + BoardUIConstants.Spacing +
                     font.MeasureString($"+{Utility.getNumberWithCommas(m.Cost)}g").X;
        maxW = Math.Max(maxW, rowW);
      }

      int total = materialCost + buildCost;
      string[] labels = { I18n.UI_InfoBoard_Total_MaterialCost(), I18n.UI_InfoBoard_Total_BuildPrice(), I18n.UI_InfoBoard_Total_TotalPayment() };
      string[] values = { $"{Utility.getNumberWithCommas(materialCost)}g", $"+{Utility.getNumberWithCommas(buildCost)}g", $"{Utility.getNumberWithCommas(total)}g" };

      for (int i = 0; i < labels.Length; i++)
      {
        float rowW = BoardUIConstants.CurrencyIconSize + BoardUIConstants.Spacing +
                     font.MeasureString(labels[i]).X + BoardUIConstants.Spacing +
                     font.MeasureString(values[i]).X;
        maxW = Math.Max(maxW, rowW);
      }

      int h = BoardUIConstants.Padding; // Top padding
      h += (int)headerDim.Y + BoardUIConstants.Spacing; // Header + Bottom spacing
      h += missing.Count * Math.Max(BoardUIConstants.MaterialIconSize, font.LineSpacing); // Materials
      h += (BoardUIConstants.DividerMargin * 4) + (BoardUIConstants.DividerHeight * 2); // Two Dividers
      h += 3 * Math.Max(BoardUIConstants.CurrencyIconSize, font.LineSpacing); // Total Rows
      h += BoardUIConstants.Padding; // Bottom Padding

      return ((int)maxW + BoardUIConstants.TotalPadding, h);
    }
  }
}
