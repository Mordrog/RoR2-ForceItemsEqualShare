using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace ForceItemsEqualShare
{
    public class PrintedItemsWatcher : NetworkBehaviour
    {
        private Dictionary<NetworkUserId, PrintedItems> userPrintedItems = new Dictionary<NetworkUserId, PrintedItems>();

        public bool CheckIfUserHasPrintedItems(NetworkUser user, PickupIndex pickupIndex)
        {
            var itemTier = ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(pickupIndex).itemIndex).tier;
            var printedItems = GetUserPrintedItems(user);
            return printedItems?.HasAnyPrintedItemOfTier(itemTier) ?? false;
        }

        public bool TryConsumeUserPrintedItem(NetworkUser user, PickupIndex pickupIndex)
        {
            var itemTier = ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(pickupIndex).itemIndex).tier;
            var printedItems = GetUserPrintedItems(user);
            return printedItems?.RemovePrintedItemIfExists(itemTier) ?? false;
        }

        public void Awake()
        {
            On.RoR2.Run.OnDestroy += Run_OnDestroy;
            On.RoR2.Run.OnServerSceneChanged += Run_OnServerSceneChanged;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
        }

        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
        {
            orig(self);

            userPrintedItems.Clear();
        }

        private void Run_OnServerSceneChanged(On.RoR2.Run.orig_OnServerSceneChanged orig, Run self, string sceneName)
        {
            orig(self, sceneName);

            userPrintedItems.Clear();
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);

            var shopTerminal = self.GetComponent<ShopTerminalBehavior>();
            var user = UsersHelper.GetUser(activator);

            if (shopTerminal && user && CheckIfCostTypeIsItem(self.costType))
            {
                if (!userPrintedItems.ContainsKey(user.id))
                {
                    userPrintedItems[user.id] = new PrintedItems();
                }
                userPrintedItems[user.id].AddPrintedItem(shopTerminal.itemTier);
            }
        }

        private PrintedItems GetUserPrintedItems(NetworkUser user)
        {
            if (user)
            {
                if (userPrintedItems.TryGetValue(user.id, out PrintedItems printedItems))
                {
                    return printedItems;
                }
            }

            return null;
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

            public bool HasAnyPrintedItemOfTier(ItemTier itemTier)
            {
                return numberOfPrintedItemsByTier.TryGetValue(itemTier, out uint value) && value > 0;
            }

            public void AddPrintedItem(ItemTier itemTier)
            {
                if (HasAnyPrintedItemOfTier(itemTier))
                    numberOfPrintedItemsByTier[itemTier] += 1;
                else
                    numberOfPrintedItemsByTier[itemTier] = 1;
            }

            public bool RemovePrintedItemIfExists(ItemTier itemTier)
            {
                if (HasAnyPrintedItemOfTier(itemTier))
                {
                    numberOfPrintedItemsByTier[itemTier] -= 1;
                    return true;
                }

                return false;
            }
        }
    }
}
