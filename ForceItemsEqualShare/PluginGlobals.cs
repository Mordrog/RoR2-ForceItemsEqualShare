using RoR2;
using System.Collections.Generic;

namespace ForceItemsEqualShare
{
    public static class PluginGlobals
    {
        public static List<ItemDef> IgnoredCalculationItems = new List<ItemDef>
        {
            RoR2Content.Items.ArtifactKey,
            RoR2Content.Items.TitanGoldDuringTP,
            RoR2Content.Items.CaptainDefenseMatrix,
            RoR2Content.Items.TPHealingNova,
            DLC3Content.Items.MasterBattery,
            DLC3Content.Items.MasterCore,
        };

        public static List<ItemDef> IgnoredPickupItems = new List<ItemDef>
        {
            RoR2Content.Items.ArtifactKey,
            RoR2Content.Items.TitanGoldDuringTP,
            RoR2Content.Items.CaptainDefenseMatrix,
            RoR2Content.Items.TPHealingNova,
            DLC3Content.Items.MasterBattery,
            DLC3Content.Items.MasterCore,

            RoR2Content.Items.ScrapWhite,
            RoR2Content.Items.ScrapGreen,
            RoR2Content.Items.ScrapRed,
            RoR2Content.Items.ScrapYellow,
        };

        public static List<ItemDef> RegeneratingGreenItems = new List<ItemDef>
        {
            DLC1Content.Items.RegeneratingScrapConsumed,
            DLC2Content.Items.LowerPricedChestsConsumed,
            DLC2Content.Items.TeleportOnLowHealthConsumed,
        };

        public static ItemDef SpeedItem = RoR2Content.Items.SprintOutOfCombat;

        //Lol is there better way to store/check stages?!?
        public static List<string> IgnoredStages = new List<string>
        {
            "bazaar",
            "arena", //void
            "moon",
            "moon2"
        };
    }
}
