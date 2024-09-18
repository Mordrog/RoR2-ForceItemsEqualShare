using BepInEx.Configuration;

namespace ForceItemsEqualShare
{
    public static class PluginConfig
    {
        public static ConfigEntry<HowToHandleItemsDisproportion>
            HowToHandleItemsDisproportion;

        public static ConfigEntry<float>
            ItemsCostsDifferenceThresholdScale,

        public static ConfigEntry<uint>
            MinItemsCostsDifferenceThreshold,
            MaxItemsCostsDifferenceThreshold,
            WhiteItemsCost,
            GreenItemsCost,
            RedItemsCost,
            BossItemsCost,
            BlueItemsCost;
    }
}
