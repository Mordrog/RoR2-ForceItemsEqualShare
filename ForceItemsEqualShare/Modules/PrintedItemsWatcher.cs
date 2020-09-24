using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Mordrog
{
    public class PrintedItemsWatcher : NetworkBehaviour
    {
        private Dictionary<MasterCatalog.MasterIndex, PrintedItems> watchedPrintedItems = 
            new Dictionary<MasterCatalog.MasterIndex, PrintedItems>();

        public void Awake()
        {
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
        }

        public bool CheckIfPlayerHasPrintedItems(CharacterMaster player, PickupIndex pickupIndex)
        {
            if (player && watchedPrintedItems.TryGetValue(player.masterIndex, out PrintedItems playerPrintedItems))
            {
                var itemTier = ItemCatalog.GetItemDef(pickupIndex.itemIndex).tier;
                return playerPrintedItems.AreTherePrintedItems(itemTier);
            }

            return false;
        }

        public bool TryConsumingPlayersPrintedItems(CharacterMaster player, PickupIndex pickupIndex)
        {
            if (player && watchedPrintedItems.TryGetValue(player.masterIndex, out PrintedItems playerPrintedItems))
            {
                var itemTier = ItemCatalog.GetItemDef(pickupIndex.itemIndex).tier;
                return playerPrintedItems.RemovePrintedItemIfExists(itemTier);
            }

            return false;
        }

        public void ClearWatchedPrintedItems()
        {
            watchedPrintedItems.Clear();
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);

            var shopTerminal = self.GetComponent<ShopTerminalBehavior>();
            var player = PlayersHelper.GetPlayer(activator.GetComponent<CharacterBody>());

            if (shopTerminal && player && CheckIfCostTypeIsItem(self.costType))
            {
                if (!watchedPrintedItems.ContainsKey(player.masterIndex))
                    watchedPrintedItems[player.masterIndex] = new PrintedItems();
                watchedPrintedItems[player.masterIndex].AddPrintedItem(shopTerminal.itemTier);
            }
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
