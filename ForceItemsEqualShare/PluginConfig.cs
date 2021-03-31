using BepInEx.Configuration;

namespace Mordrog
{
    public static class PluginConfig
    {
        public static ConfigEntry<HowToHandleItemDisproportion>
            HowToHandleItemDisproportion;

        public static ConfigEntry<float>
            ScaleItemsCostsDifference;

        public static ConfigEntry<uint>
            MinItemsCostsDifference,
            MaxItemsCostsDifference,
            WhiteItemsCost,
            GreenItemsCost,
            RedItemsCost,
            BossItemsCost,
            BlueItemsCost;
    }
}
