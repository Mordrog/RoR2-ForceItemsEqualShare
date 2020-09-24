using RoR2;
using System;
using UnityEngine.Networking;

namespace Mordrog
{
    public class PickupItemsPermissionController : NetworkBehaviour
    {
        private PrintedItemsWatcher printedItemsWatcher;
        private CurrentStageWatcher currentStageWatcher;

        public void Awake()
        {
            printedItemsWatcher = base.gameObject.AddComponent<PrintedItemsWatcher>();
            currentStageWatcher = base.gameObject.AddComponent<CurrentStageWatcher>();

            currentStageWatcher.OnCurrentStageChanged += CurrentStageWatcher_OnCurrentStageChanged;
            On.RoR2.GenericPickupController.AttemptGrant += GenericPickupController_AttemptGrant;
            On.RoR2.GenericPickupController.OnInteractionBegin += GenericPickupController_OnInteractionBegin;
        }

        private void GenericPickupController_OnInteractionBegin(On.RoR2.GenericPickupController.orig_OnInteractionBegin orig, GenericPickupController self, Interactor activator)
        {
            // This method is used only for messages
            var body = activator.GetComponent<CharacterBody>();
            var player = PlayersHelper.GetPlayer(body);

            if (!player ||
                !CheckIfCurrentStageQualifyForSharing() ||
                !CheckIfItemQualifyForSharing(self.pickupIndex) ||
                printedItemsWatcher.CheckIfPlayerHasPrintedItems(player, self.pickupIndex) ||
                !CheckIfPlayerHasTooManyItemsCosts(player))
            {
                orig(self, activator);
                return;
            }

            var user = PlayersHelper.GetUser(player);
            var playerWithLeastItems = PlayersItemsCostsCounter.GetPlayerWithLeastItemsCosts();
            var userWithLeastItems = PlayersHelper.GetUser(playerWithLeastItems);
            if (user && userWithLeastItems)
                ChatHelper.PlayerHaveTooManyItems(user.userName, userWithLeastItems.userName);
        }

        private void CurrentStageWatcher_OnCurrentStageChanged()
        {
            printedItemsWatcher.ClearWatchedPrintedItems();
        }

        private void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            var player = PlayersHelper.GetPlayer(body);

            if (!player ||
                !CheckIfCurrentStageQualifyForSharing() ||
                !CheckIfItemQualifyForSharing(self.pickupIndex) ||
                printedItemsWatcher.TryConsumingPlayersPrintedItems(player, self.pickupIndex))
            {
                orig(self, body);
                return;
            }

            if (!CheckIfPlayerHasTooManyItemsCosts(player))
                orig(self, body);
        }

        private bool CheckIfCurrentStageQualifyForSharing()
        {
            return !PluginGlobals.IgnoredStages.Contains(currentStageWatcher.currentStage);
        }

        private bool CheckIfItemQualifyForSharing(PickupIndex pickupIndex)
        {
            var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

            return pickupDef.itemIndex != ItemIndex.None &&
                   !pickupDef.isLunar &&
                   !PluginGlobals.IgnoredPickupItems.Contains(pickupDef.itemIndex);
        }

        private bool CheckIfPlayerHasTooManyItemsCosts(CharacterMaster player)
        {
            var playerItemsCosts = PlayersItemsCostsCounter.GetItemsCosts(player.inventory);
            var playerLeastItemsCosts = PlayersItemsCostsCounter.GetPlayersLeastItemsCosts();

            return CheckIfItemsCostsDifferenceExceedThreshold(playerItemsCosts, playerLeastItemsCosts);
        }

        private bool CheckIfItemsCostsDifferenceExceedThreshold(uint playerItemsCosts, uint playerLeastItemsCosts)
        {
            uint itemsCostsDifference = playerItemsCosts - playerLeastItemsCosts;

            return itemsCostsDifference >= GetItemsCostsThreshold(playerItemsCosts);
        }

        private uint GetItemsCostsThreshold(uint playerItemsCosts)
        {
            uint itemsCostsThreshold = 0;

            if (PluginConfig.ScaleItemsCostsDifference.Value >= 0 && PluginConfig.ScaleItemsCostsDifference.Value <= 1)
                itemsCostsThreshold = (uint)(playerItemsCosts * PluginConfig.ScaleItemsCostsDifference.Value);

            if (PluginConfig.MinItemsCostsDifference.Value <= PluginConfig.MaxItemsCostsDifference.Value)
            {
                itemsCostsThreshold = Math.Max(itemsCostsThreshold, PluginConfig.MinItemsCostsDifference.Value);
                itemsCostsThreshold = Math.Min(itemsCostsThreshold, PluginConfig.MaxItemsCostsDifference.Value);
            }

            return itemsCostsThreshold;
        }
    }
}
