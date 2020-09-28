using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    class PingedItemsWatcher : NetworkBehaviour
    {
        // int stands for Unity instance id
        private Dictionary<MasterCatalog.MasterIndex, int> pingedItems = new Dictionary<MasterCatalog.MasterIndex, int>();

        public void Awake()
        {
            On.RoR2.PingerController.SetCurrentPing += PingerController_SetCurrentPing;
        }

        private void PingerController_SetCurrentPing(On.RoR2.PingerController.orig_SetCurrentPing orig, PingerController self, PingerController.PingInfo newPingInfo)
        {
            orig(self, newPingInfo);

            var player = self.GetComponent<CharacterMaster>();
            var item = newPingInfo.targetGameObject?.GetComponent<GenericPickupController>();
            if (item && player)
            {
                pingedItems[player.masterIndex] = item.GetInstanceID();
            }
            else if (player)
            {
                pingedItems.Remove(player.masterIndex);
            }
        }

        public bool CheckIfItemPingedByPlayer(CharacterMaster player, GenericPickupController pickupController)
        {
            return player && pingedItems.TryGetValue(player.masterIndex, out var value) && value == pickupController.GetInstanceID();
        }

        public bool TryRemoveItemPingedByPlayer(CharacterMaster player, GenericPickupController pickupController)
        {
            if (CheckIfItemPingedByPlayer(player, pickupController))
            {
                pingedItems.Remove(player.masterIndex);
                return true;
            }

            return false;
        }

        public void ClearWatchedPingedItems()
        {
            pingedItems.Clear();
        }
    }
}
