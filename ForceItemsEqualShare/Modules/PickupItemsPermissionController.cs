using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace ForceItemsEqualShare
{
    public enum HowToHandleItemsDisproportion
    {
        GiveRandomItemToLowestCostsPlayer,
        PreventBiggestCostsPlayerPickup
    }

    public class PickupItemsPermissionController : NetworkBehaviour
    {
        private PrintedItemsWatcher printedItemsWatcher;
        private PingedItemsWatcher pingedItemsWatcher;

        public void Awake()
        {
            printedItemsWatcher = gameObject.AddComponent<PrintedItemsWatcher>();
            pingedItemsWatcher = gameObject.AddComponent<PingedItemsWatcher>();

            On.RoR2.GenericPickupController.AttemptGrant += GenericPickupController_AttemptGrant;
            On.RoR2.GenericPickupController.OnInteractionBegin += GenericPickupController_OnInteractionBegin;
        }

        // If "orig" method is fired, GenericPickupController_AttemptGrant will happen next
        private void GenericPickupController_OnInteractionBegin(On.RoR2.GenericPickupController.orig_OnInteractionBegin orig, GenericPickupController self, Interactor activator)
        {
            if (!CheckIfCurrentStageQualifyForSharing() ||
                !CheckIfItemQualifyForSharing(self.pickupIndex))
            {
                orig(self, activator);
                return;
            }

            var user = UsersHelper.GetUser(activator);

            // get item
            if (printedItemsWatcher.CheckIfUserHasPrintedItems(user, self.pickupIndex) ||
                CheckIfUserCanPickItem(user))
            {
                orig(self, activator);
                return;
            }
            // get item and boost players with item
            else if (PluginConfig.HowToHandleItemsDisproportion.Value == HowToHandleItemsDisproportion.GiveRandomItemToLowestCostsPlayer)
            {
                orig(self, activator);
                return;
            }
            // share item
            else if (pingedItemsWatcher.TryConsumeItemPingedByUser(user, self))
            {
                var userWithLeastItems = InventoryCostMath.GetUserWithLeastInventoryCosts().user;
                var userWithLeastItemsInteractor = UsersHelper.GetUserInteractor(userWithLeastItems);

                if (userWithLeastItemsInteractor)
                    self.OnInteractionBegin(userWithLeastItemsInteractor);
            }
            // can't pick item
            else if (user)
            {
                ChatHelper.PlayerHasTooManyItems(user.userName);
            }
        }

        // Only happens if original GenericPickupController_OnInteractionBegin is fired
        private void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            if (!CheckIfCurrentStageQualifyForSharing() ||
                !CheckIfItemQualifyForSharing(self.pickupIndex))
            {
                orig(self, body);
                return;
            }

            var user = UsersHelper.GetUser(body);

            // Allow user to pick up their printed item, even if they exceeds inventory costs threshold
            if (printedItemsWatcher.TryConsumeUserPrintedItem(user, self.pickupIndex) ||
                CheckIfUserCanPickItem(user))
            {
                orig(self, body);
                return;
            }
            else if (PluginConfig.HowToHandleItemsDisproportion.Value == HowToHandleItemsDisproportion.GiveRandomItemToLowestCostsPlayer)
            {
                foreach (var otherUser in InventoryCostMath.GetUsersWithLessInventoryCosts(user))
                {
                    BoostUserWithRandomItem(otherUser);
                }

                orig(self, body);
                return;
            }
        }

        // method copied from RoR2.Inventory::GiveRandomItems
        private void BoostUserWithRandomItem(NetworkUser user)
        {
            if (!user || !user.master || !user.master.inventory)
                return;

            var inventory = user.master.inventory;

            try
            {
                WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
                weightedSelection.AddChoice(Run.instance.availableTier1DropList, 100f);
                weightedSelection.AddChoice(Run.instance.availableTier2DropList, 20f);

                List<PickupIndex> list = weightedSelection.Evaluate(UnityEngine.Random.value);
                PickupDef pickupDef = PickupCatalog.GetPickupDef(list[UnityEngine.Random.Range(0, list.Count)]);
                inventory.GiveItem((pickupDef != null) ? pickupDef.itemIndex : ItemIndex.None, 1);

                ChatHelper.PlayerBoostedWithItem(user.userName, pickupDef.nameToken, pickupDef.baseColor);
            }
            catch (System.ArgumentException)
            {
            }
        }

        private bool CheckIfCurrentStageQualifyForSharing()
        {
            return !PluginGlobals.IgnoredStages.Contains(SceneCatalog.GetSceneDefForCurrentScene().baseSceneName);
        }

        private bool CheckIfItemQualifyForSharing(PickupIndex pickupIndex)
        {
            var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

            return pickupDef.itemIndex != ItemIndex.None &&
                   !pickupDef.isLunar &&
                   !PluginGlobals.IgnoredPickupItems.Find(i => i.itemIndex == pickupDef.itemIndex);
        }

        private bool CheckIfUserCanPickItem(NetworkUser user)
        {
            if (!user || !user.master)
                return false;

            var userInventory = user.master.inventory;
            return userInventory && !InventoryCostMath.CheckIfInventoryHasTooMuchInventoryCost(userInventory);
        }
    }
}
