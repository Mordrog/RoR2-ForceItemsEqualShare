using RoR2;
using System;

namespace Mordrog
{
    public static class InventoryCostCounter
    {
        public static (NetworkUser user, int cost) GetUserWithLeastItemsCosts(bool ignoreDeadUsers = true)
        {
            int leastItemsCosts = int.MaxValue;
            NetworkUser userWithLeastItemsCosts = null;

            foreach (var user in UsersHelper.GetAllUsers())
            {
                if (!user.master || !user.master.inventory)
                    continue;

                if (ignoreDeadUsers && user.master.IsDeadAndOutOfLivesServer())
                    continue;

                var playerItemsCost = GetInventoryCost(user.master.inventory);

                if (playerItemsCost < leastItemsCosts)
                {
                    leastItemsCosts = playerItemsCost;
                    userWithLeastItemsCosts = user;
                }
            }

            return (userWithLeastItemsCosts, leastItemsCosts);
        }

        public static int GetInventoryCost(NetworkUser user)
        {
            if (!user || !user.master || !user.master.inventory)
                return 0;

            return GetInventoryCost(user.master.inventory);
        }

        public static int GetInventoryCost(Inventory inventory)
        {
            int itemsCosts = 0;

            foreach (var itemIndex in inventory.itemAcquisitionOrder)
            {
                if (PluginGlobals.IgnoredCalculationItems.Contains(itemIndex))
                    continue;

                var numberOfStacks = inventory.GetItemCount(itemIndex);
                itemsCosts += ItemCostEvaluation.GetItemCostEvaluation(itemIndex, numberOfStacks);
            }

            return itemsCosts;
        }

        public static bool CheckIfInventoryHasTooMuchInventoryCost(Inventory inventory)
        {
            var inventoryCost = InventoryCostCounter.GetInventoryCost(inventory);

            return CheckIfInventoryCostExceedThreshold(inventoryCost);
        }

        public static bool CheckIfInventoryCostExceedThreshold(int inventoryCost)
        {
            var leastInventoryCost = InventoryCostCounter.GetUserWithLeastItemsCosts().cost;

            int inventoryCostDifference = inventoryCost - Math.Min(leastInventoryCost, inventoryCost);

            return inventoryCostDifference >= GetInventoryCostThreshold(inventoryCost);
        }

        public static int GetInventoryCostThreshold(int inventoryCost)
        {
            int itemsCostsThreshold = 0;

            if (PluginConfig.ScaleItemsCostsDifference.Value >= 0 && PluginConfig.ScaleItemsCostsDifference.Value <= 1)
                itemsCostsThreshold = (int)(inventoryCost * PluginConfig.ScaleItemsCostsDifference.Value);

            if (PluginConfig.MinItemsCostsDifference.Value <= PluginConfig.MaxItemsCostsDifference.Value)
            {
                itemsCostsThreshold = Math.Max(itemsCostsThreshold, (int)PluginConfig.MinItemsCostsDifference.Value);
                itemsCostsThreshold = Math.Min(itemsCostsThreshold, (int)PluginConfig.MaxItemsCostsDifference.Value);
            }

            return itemsCostsThreshold;
        }
    }
}
