using RoR2;

namespace Mordrog
{
    public static class ItemCostEvaluation
    {
        public static uint GetItemCostEvaluation(ItemIndex item, uint numberOfStacks = 1)
        {
            return numberOfStacks * GetItemTierCostEvaluation(ItemCatalog.GetItemDef(item).tier);
        }

        public static uint GetItemTierCostEvaluation(ItemTier itemTier)
        {
            switch (itemTier)
            {
                case ItemTier.Tier1:
                    return PluginConfig.WhiteItemsCost.Value;
                case ItemTier.Tier2:
                    return PluginConfig.GreenItemsCost.Value;
                case ItemTier.Tier3:
                    return PluginConfig.RedItemsCost.Value;
                case ItemTier.Boss:
                    return PluginConfig.BossItemsCost.Value;
                case ItemTier.Lunar:
                    return PluginConfig.BlueItemsCost.Value;
                case ItemTier.NoTier:
                default:
                    return 0;
            }
        }
    }
}
