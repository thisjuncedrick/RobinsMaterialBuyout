using RobinsMaterialBuyout.Utilities;

namespace RobinsMaterialBuyout.Models
{
  internal record BuyoutModel
  {
    public List<BuyoutMaterial> MissingMaterials { get; init; }
    public int MaterialCost { get; init; }
    public int TotalCost { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public BuyoutModel(List<BuyoutMaterial> missing, int baseBuildCost)
    {
      MissingMaterials = missing;
      MaterialCost = missing.Sum(m => m.Cost);
      TotalCost = MaterialCost + baseBuildCost;

      var (w, h) = LayoutMeasurer.Measure(missing, MaterialCost, baseBuildCost);
      Width = w;
      Height = h;
    }
  }
}