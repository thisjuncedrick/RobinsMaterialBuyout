using RobinsMaterialBuyout.Models;

using StardewValley;

namespace RobinsMaterialBuyout.Services
{
  internal class MaterialCostService
  {
    private static readonly Dictionary<string, int> PriceCache = new();

    public static void ClearCache()
    {
      PriceCache.Clear();
      ModEntry.Log("Material price cache invalidated.");
    }

    public static List<BuyoutMaterial> GetMissingMaterials(List<Item> ingredients, bool useBasePrice)
    {
      var missing = new List<BuyoutMaterial>();
      foreach (var item in ingredients)
      {
        int have = Game1.player.Items.CountId(item.QualifiedItemId);
        int need = item.Stack - have;

        if (need > 0)
        {
          int unitPrice = GetUnitPrice(item.QualifiedItemId, useBasePrice, item.Name);
          missing.Add(new BuyoutMaterial(item, need, need * unitPrice));
        }
      }
      return missing;
    }

    private static int GetUnitPrice(string id, bool useBasePrice, string itemName)
    {
      string cacheKey = $"{id}_{useBasePrice}";
      if (PriceCache.TryGetValue(cacheKey, out int cached)) 
      {
        ModEntry.Log($"Cache hit for {itemName} ({(useBasePrice ? "base price" : "adjusted price")}): {cached:N0}g");
        return cached;
      }

      var item = ItemRegistry.Create(id);
      int price = useBasePrice
          ? (ItemRegistry.GetData(id)?.RawData as StardewValley.GameData.Objects.ObjectData)?.Price ?? item.sellToStorePrice()
          : Utility.getSellToStorePriceOfItem(item);

      ModEntry.Log($"Caching {itemName} ({(useBasePrice ? "base price" : "adjusted price")}): {price:N0}g");
      return PriceCache[cacheKey] = price;
    }

  }
}
