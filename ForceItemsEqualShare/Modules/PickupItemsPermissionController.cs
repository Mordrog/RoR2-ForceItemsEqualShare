using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    public class PickupItemsPermissionController : NetworkBehaviour
    {
        private PrintedItemsWatcher printedItemsWatcher;
        private PingedItemsWatcher pingedItemsWatcher;

        public void Awake()
        {
            printedItemsWatcher = base.gameObject.AddComponent<PrintedItemsWatcher>();
            pingedItemsWatcher = base.gameObject.AddComponent<PingedItemsWatcher>();

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
            // share item
            else if (pingedItemsWatcher.TryConsumeItemPingedByUser(user, self))
            {
                var userWithLeastItems = InventoryCostCounter.GetUserWithLeastItemsCosts().user;
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

        private void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            if (!CheckIfCurrentStageQualifyForSharing() || 
                !CheckIfItemQualifyForSharing(self.pickupIndex))
            {
                orig(self, body);
                return;
            }

            var user = UsersHelper.GetUser(body);

            if (printedItemsWatcher.TryConsumeUserPrintedItem(user, self.pickupIndex) ||
                CheckIfUserCanPickItem(user))
            {
                orig(self, body);
                return;
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
                   !PluginGlobals.IgnoredPickupItems.Contains(pickupIndex.itemIndex);
        }

        private bool CheckIfUserCanPickItem(NetworkUser user)
        {
            if (!user || !user.master)
                return false;

            var userInventory = user.master.inventory;

            return userInventory && !InventoryCostCounter.CheckIfInventoryHasTooMuchInventoryCost(userInventory);
        }
    }
}
