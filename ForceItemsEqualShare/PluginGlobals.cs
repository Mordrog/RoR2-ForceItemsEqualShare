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
            RoR2Content.Items.TonicAffliction,
            RoR2Content.Items.CaptainDefenseMatrix,
            RoR2Content.Items.DrizzlePlayerHelper,
            RoR2Content.Items.MonsoonPlayerHelper,
            RoR2Content.Items.InvadingDoppelganger,
        };

        public static List<ItemDef> IgnoredPickupItems = new List<ItemDef>
        {
            RoR2Content.Items.ArtifactKey,
            RoR2Content.Items.ExtraLifeConsumed,
            RoR2Content.Items.TitanGoldDuringTP,
            RoR2Content.Items.TonicAffliction,
            RoR2Content.Items.CaptainDefenseMatrix,

            RoR2Content.Items.ScrapWhite,
            RoR2Content.Items.ScrapGreen,
            RoR2Content.Items.ScrapRed,
            RoR2Content.Items.ScrapYellow,
        };

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
