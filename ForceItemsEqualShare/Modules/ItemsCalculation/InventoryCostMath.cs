using RoR2;
using System;
using System.Collections.Generic;

namespace ForceItemsEqualShare
{
    public static class InventoryCostMath
    {
        public static (NetworkUser user, int cost) GetUserWithLeastInventoryCosts(bool ignoreDeadUsers = true)
        {
            int leastInventoryCosts = int.MaxValue;
            NetworkUser userWithLeastInventoryCosts = null;

            foreach (var user in UsersHelper.GetAllUsers())
            {
                if (!user.master || !user.master.inventory)
                    continue;

                if (ignoreDeadUsers && user.master.IsDeadAndOutOfLivesServer())
                    continue;

                var userInventoryCost = GetInventoryCost(user.master.inventory);
                if (userInventoryCost < leastInventoryCosts)
                {
                    leastInventoryCosts = userInventoryCost;
                    userWithLeastInventoryCosts = user;
                }
            }

            return (userWithLeastInventoryCosts, leastInventoryCosts);
        }

        public static int GetInventoryCost(NetworkUser user)
        {
            if (!user || !user.master || !user.master.inventory)
            {
                return 0;
            }

            return GetInventoryCost(user.master.inventory);
        }

        public static int GetInventoryCost(Inventory inventory)
        {
            int InventoryCosts = 0;

            foreach (var itemIndex in inventory.itemAcquisitionOrder)
            {
                if (PluginGlobals.IgnoredCalculationItems.Find(i => i.itemIndex == itemIndex))
                {
                    continue;
                }

                var numberOfStacks = inventory.GetItemCount(itemIndex);
                InventoryCosts += ItemCostEvaluation.GetItemCostEvaluation(itemIndex, numberOfStacks);
            }

            return InventoryCosts;
        }

        public static bool CheckIfInventoryHasTooMuchInventoryCost(Inventory inventory)
        {
            var inventoryCost = GetInventoryCost(inventory);

            return CheckIfInventoryCostExceedThreshold(inventoryCost);
        }

        public static bool CheckIfInventoryCostExceedThreshold(int inventoryCost)
        {
            var leastInventoryCost = GetUserWithLeastInventoryCosts().cost;
            int inventoryCostDifference = inventoryCost - Math.Min(leastInventoryCost, inventoryCost);

            return inventoryCostDifference >= GetInventoryDifferenceCostThreshold(inventoryCost);
        }

        public static int GetInventoryDifferenceCostThreshold(int inventoryCost)
        {
            int itemsCostsDifferenceThreshold = 0;

            if (PluginConfig.ItemsCostsDifferenceThresholdScale.Value >= 0 && PluginConfig.ItemsCostsDifferenceThresholdScale.Value <= 1)
            {
                itemsCostsDifferenceThreshold = (int)(inventoryCost * PluginConfig.ItemsCostsDifferenceThresholdScale.Value);
            }

            if (PluginConfig.MinItemsCostsDifferenceThreshold.Value <= PluginConfig.MaxItemsCostsDifferenceThreshold.Value)
            {
                itemsCostsDifferenceThreshold = Math.Clamp(itemsCostsDifferenceThreshold, (int)PluginConfig.MinItemsCostsDifferenceThreshold.Value, (int)PluginConfig.MaxItemsCostsDifferenceThreshold.Value);
            }

            return itemsCostsDifferenceThreshold;
        }
    }
}
