using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    public class PickupItemsPermissionController : NetworkBehaviour
    {
        private PrintedItemsWatcher printedItemsWatcher;
        private CurrentStageWatcher currentStageWatcher;
        private PingedItemsWatcher pingedItemsWatcher;

        public void Awake()
        {
            printedItemsWatcher = base.gameObject.AddComponent<PrintedItemsWatcher>();
            currentStageWatcher = base.gameObject.AddComponent<CurrentStageWatcher>();
            pingedItemsWatcher = base.gameObject.AddComponent<PingedItemsWatcher>();

            currentStageWatcher.OnCurrentStageChanged += CurrentStageWatcher_OnCurrentStageChanged;
            On.RoR2.GenericPickupController.AttemptGrant += GenericPickupController_AttemptGrant;
            On.RoR2.GenericPickupController.OnInteractionBegin += GenericPickupController_OnInteractionBegin;
        }

        private void CurrentStageWatcher_OnCurrentStageChanged()
        {
            printedItemsWatcher.ClearWatchedPrintedItems();
            pingedItemsWatcher.ClearWatchedPingedItems();
        }

        private void GenericPickupController_OnInteractionBegin(On.RoR2.GenericPickupController.orig_OnInteractionBegin orig, GenericPickupController self, Interactor activator)
        {
            var player = PlayersHelper.GetPlayer(activator.GetComponent<CharacterBody>());
            var playerWithLeastItems = PlayersItemsCostsCounter.GetPlayerWithLeastItemsCosts();
            var playerWithLeastItemsInteractor = PlayersHelper.GetPlayersInteractor(playerWithLeastItems);

            // get item
            if (CheckIfPlayerCanPickItem(player, self.pickupIndex) ||
                printedItemsWatcher.CheckIfPlayerHasPrintedItems(player, self.pickupIndex))
            {
                orig(self, activator);
                return;
            }
            // share item
            if (playerWithLeastItemsInteractor &&
                CheckIfItemQualifyForSharing(self.pickupIndex) &&
                pingedItemsWatcher.TryRemoveItemPingedByPlayer(player, self))
            {
                self.OnInteractionBegin(playerWithLeastItemsInteractor);
                return;
            }
            // can't pick item
            else
            {
                var user = PlayersHelper.GetUser(player);

                if (user)
                    ChatHelper.PlayerHasTooManyItems(user.userName);
            }
        }

        private void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            var player = PlayersHelper.GetPlayer(body);

            if (CheckIfPlayerCanPickItem(player, self.pickupIndex) ||
                printedItemsWatcher.TryConsumePlayersPrintedItem(player, self.pickupIndex))
            {
                orig(self, body);
                return;
            }
        }

        private bool CheckIfPlayerCanPickItem(CharacterMaster player, PickupIndex pickupIndex)
        {
            return !player ||
                !CheckIfCurrentStageQualifyForSharing() ||
                !CheckIfItemQualifyForSharing(pickupIndex) ||
                !CheckIfPlayerHasTooManyItemsCosts(player);
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
