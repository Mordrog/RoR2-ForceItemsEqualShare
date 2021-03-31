using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    public class PrintedItemsWatcher : NetworkBehaviour
    {
        private Dictionary<NetworkUserId, PrintedItems> watchedPrintedItems = new Dictionary<NetworkUserId, PrintedItems>();

        public void Awake()
        {
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
        {
            orig(self);

            watchedPrintedItems.Clear();
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            watchedPrintedItems.Clear();
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);

            var shopTerminal = self.GetComponent<ShopTerminalBehavior>();
            var user = UsersHelper.GetUser(activator);

            if (shopTerminal && user && CheckIfCostTypeIsItem(self.costType))
            {
                if (!watchedPrintedItems.ContainsKey(user.id))
                    watchedPrintedItems[user.id] = new PrintedItems();
                watchedPrintedItems[user.id].AddPrintedItem(shopTerminal.itemTier);
            }
        }

        public bool CheckIfUserHasPrintedItems(NetworkUser user, PickupIndex pickupIndex)
        {
            if (user && watchedPrintedItems.TryGetValue(user.id, out PrintedItems userPrintedItems))
            {
                
                var itemTier = ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(pickupIndex).itemIndex).tier;
                return userPrintedItems.AreTherePrintedItems(itemTier);
            }

            return false;
        }

        public bool TryConsumeUserPrintedItem(NetworkUser user, PickupIndex pickupIndex)
        {
            if (user && watchedPrintedItems.TryGetValue(user.id, out PrintedItems userPrintedItems))
            {
                var itemTier = ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(pickupIndex).itemIndex).tier;
                return userPrintedItems.RemovePrintedItemIfExists(itemTier);
            }

            return false;
        }

        private bool CheckIfCostTypeIsItem(CostTypeIndex costType)
        {
            return costType == CostTypeIndex.WhiteItem ||
                   costType == CostTypeIndex.GreenItem ||
                   costType == CostTypeIndex.RedItem ||
                   costType == CostTypeIndex.BossItem;
        }

        internal class PrintedItems
        {
            private SortedDictionary<ItemTier, uint> numberOfPrintedItemsByTier = new SortedDictionary<ItemTier, uint>();

            public bool AreTherePrintedItems(ItemTier itemTier)
            {
                return numberOfPrintedItemsByTier.TryGetValue(itemTier, out uint value) && value > 0;
            }

            public void AddPrintedItem(ItemTier itemTier)
            {
                if (AreTherePrintedItems(itemTier))
                    numberOfPrintedItemsByTier[itemTier] += 1;
                else
                    numberOfPrintedItemsByTier[itemTier] = 1;
            }

            public bool RemovePrintedItemIfExists(ItemTier itemTier)
            {
                if (AreTherePrintedItems(itemTier))
                {
                    numberOfPrintedItemsByTier[itemTier] -= 1;
                    return true;
                }

                return false;
            }
        }
    }
}
