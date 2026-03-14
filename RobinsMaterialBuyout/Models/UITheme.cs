using Microsoft.Xna.Framework;
using StardewValley;

namespace RobinsMaterialBuyout.Models
{
  internal record UITheme
  {
    public Color BoxColor { get; init; }
    public Color TextColor { get; init; }
    public Color CostColor { get; init; }
    public Color DividerColor { get; init; }
    public float ShadowAlpha { get; init; }

    public UITheme(bool magical)
    {
      if (magical)
      {
        BoxColor = Color.RoyalBlue;
        TextColor = Color.PaleGoldenrod;
        CostColor = Color.Cyan;
        DividerColor = Color.PaleGoldenrod * 0.5f;
        ShadowAlpha = 0f;
      }
      else
      {
        BoxColor = Color.White;
        TextColor = Game1.textColor;
        CostColor = Color.Maroon;
        DividerColor = Color.Gray * 0.5f;
        ShadowAlpha = 0.75f;
      }
    }
  }
}
