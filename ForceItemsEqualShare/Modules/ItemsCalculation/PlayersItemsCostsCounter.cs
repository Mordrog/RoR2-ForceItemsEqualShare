using RoR2;

namespace Mordrog
{
    public static class PlayersItemsCostsCounter
    {
        public static uint GetPlayersLeastItemsCosts()
        {
            uint leastItemsCosts = uint.MaxValue;

            foreach (var player in PlayersHelper.GetAllPlayers())
            {
                if (player.IsDeadAndOutOfLivesServer())
                    continue;

                var playerItemsCost = GetItemsCosts(player.inventory);

                leastItemsCosts = (playerItemsCost < leastItemsCosts) ? playerItemsCost : leastItemsCosts;
            }

            return leastItemsCosts;
        }

        public static uint GetItemsCosts(Inventory inventory)
        {
            uint itemsCosts = 0;

            foreach (var item in inventory.itemAcquisitionOrder)
            {
                if (PluginGlobals.IgnoredCalculationItems.Contains(item))
                    continue;

                var numberOfStacks = (uint)inventory.GetItemCount(item);
                itemsCosts += ItemCostEvaluation.GetItemCostEvaluation(item, numberOfStacks);
            }

            return itemsCosts;
        }
    }
}
