using RoR2;

namespace ForceItemsEqualShare
{
    public static class ItemCostEvaluation
    {
        public static int GetItemCostEvaluation(ItemDef item, int numberOfStacks = 1)
        {
            var tier = item.tier;
            if (PluginGlobals.RegeneratingGreenItems.Contains(item))
            {
                tier = ItemTier.Tier2;
            }

            return numberOfStacks * GetItemTierCostEvaluation(tier);
        }

        public static int GetItemTierCostEvaluation(ItemTier itemTier)
        {
            switch (itemTier)
            {
                case ItemTier.Tier1:
                case ItemTier.VoidTier1:
                    return (int)PluginConfig.WhiteItemsCost.Value;
                case ItemTier.Tier2:
                case ItemTier.VoidTier2:
                    return (int)PluginConfig.GreenItemsCost.Value;
                case ItemTier.Tier3:
                case ItemTier.VoidTier3:
                    return (int)PluginConfig.RedItemsCost.Value;
                case ItemTier.Boss:
                case ItemTier.VoidBoss:
                    return (int)PluginConfig.BossItemsCost.Value;
                case ItemTier.Lunar:
                    return (int)PluginConfig.BlueItemsCost.Value;
                case ItemTier.FoodTier:
                    return (int)PluginConfig.MealItemsCost.Value;
                case ItemTier.NoTier:
                default:
                    return 0;
            }
        }
    }
}
