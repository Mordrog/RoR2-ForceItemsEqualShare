using RoR2;

namespace Mordrog
{
    public static class ItemCostEvaluation
    {
        public static int GetItemCostEvaluation(ItemIndex item, int numberOfStacks = 1)
        {
            return numberOfStacks * GetItemTierCostEvaluation(ItemCatalog.GetItemDef(item).tier);
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
                case ItemTier.NoTier:
                default:
                    return 0;
            }
        }
    }
}
