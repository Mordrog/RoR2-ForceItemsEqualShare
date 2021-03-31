using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    class PingedItemsWatcher : NetworkBehaviour
    {
        // int stands for Unity instance id
        private Dictionary<NetworkUserId, int> watchedPingedItems = new Dictionary<NetworkUserId, int>();

        public void Awake()
        {
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
            On.RoR2.PingerController.SetCurrentPing += PingerController_SetCurrentPing;
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
        {
            orig(self);

            watchedPingedItems.Clear();
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            watchedPingedItems.Clear();
        }

        private void PingerController_SetCurrentPing(On.RoR2.PingerController.orig_SetCurrentPing orig, PingerController self, PingerController.PingInfo newPingInfo)
        {
            orig(self, newPingInfo);

            var user = UsersHelper.GetUser(self);
            var item = newPingInfo.targetGameObject?.GetComponent<GenericPickupController>();

            if (item && user)
            {
                watchedPingedItems[user.id] = item.GetInstanceID();
            }
            else if (user)
            {
                watchedPingedItems.Remove(user.id);
            }
        }

        public bool CheckIfItemPingedByUser(NetworkUser user, GenericPickupController pickupController)
        {
            return user && watchedPingedItems.TryGetValue(user.id, out var value) && value == pickupController.GetInstanceID();
        }

        public bool TryConsumeItemPingedByUser(NetworkUser user, GenericPickupController pickupController)
        {
            if (CheckIfItemPingedByUser(user, pickupController))
            {
                watchedPingedItems.Remove(user.id);
                return true;
            }

            return false;
        }
    }
}
