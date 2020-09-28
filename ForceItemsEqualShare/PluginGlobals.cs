using RoR2;
using System.Collections.Generic;

namespace Mordrog
{
    public static class PluginGlobals
    {
        public static string TooManyItemsMessage = "You have too many items, share some!";

        public static List<ItemIndex> IgnoredCalculationItems = new List<ItemIndex>
        {
            ItemIndex.None,
            ItemIndex.ArtifactKey,
            ItemIndex.ExtraLifeConsumed,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.TonicAffliction,
            ItemIndex.CaptainDefenseMatrix
        };

        public static List<ItemIndex> IgnoredPickupItems = new List<ItemIndex>
        {
            ItemIndex.None,
            ItemIndex.ArtifactKey,
            ItemIndex.ExtraLifeConsumed,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.TonicAffliction,
            ItemIndex.CaptainDefenseMatrix,

            ItemIndex.ScrapWhite,
            ItemIndex.ScrapGreen,
            ItemIndex.ScrapRed,
            ItemIndex.ScrapYellow
        };

        //Lol is there better way to store/check stages?!?
        public static List<string> IgnoredStages = new List<string>
        {
            "bazaar",
            "arena", //void
            "moon",
        };
    }
}
