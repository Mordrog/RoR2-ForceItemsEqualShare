using BepInEx;
using R2API.Utils;

namespace ForceItemsEqualShare
{
    [BepInDependency("com.bepis.r2api")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class ForceItemsEqualSharePlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.3.2";
        public const string ModName = "ForceItemsEqualShare";
        public const string ModGuid = "com.Mordrog.ForceItemsEqualShare";

        public ForceItemsEqualSharePlugin()
        {
            InitConfig();
        }

        public void Awake()
        {
            gameObject.AddComponent<PickupItemsPermissionController>();
        }

        private void InitConfig()
        {
            PluginConfig.HowToHandleItemsDisproportion = Config.Bind<HowToHandleItemsDisproportion>(
                "Settings",
                "HowToHandleItemsDisproportion",
                HowToHandleItemsDisproportion.GiveRandomItemToLowestCostsPlayer,
                "Way to handle items disproportion between player with loweset and biggest item costs"
            );

            PluginConfig.ItemsCostsDifferenceThresholdScale = Config.Bind<float>(
                "Settings",
                "ItemsCostsDifferenceThresholdScale",
                0.3f,
                "The items costs difference threshold scale for player picking item against the player with the lowest number of items costs.\n" +
                "Too many items formula: Clamp(PlayersItemsCost * ItemsCostsDifferenceThresholdScale, MinItemsCostsDifferenceThreshold, MaxItemsCostsDifferenceThreshold) <= PlayersItemsCost - LeastPlayerItemsCost\n" +
                "Have to be between 0 and 1"
            );

            PluginConfig.MinItemsCostsDifferenceThreshold = Config.Bind<uint>(
                "Settings",
                "MinItemsCostsDifferenceThreshold",
                5,
                "The min items costs difference threshold for player picking item against the player with the lowest number of items costs..\n" +
                "Have to be equal or lesser then MaxItemsCostsDifferenceThreshold"
            );

            PluginConfig.MaxItemsCostsDifferenceThreshold = Config.Bind<uint>(
                "Settings",
                "MaxItemsCostsDifferenceThreshold",
                15,
                "The max items costs difference threshold for player picking item against the player with the lowest number of items costs.\n" +
                "Have to be equal or bigger then MinItemsCostsDifferenceThreshold"
            );

            PluginConfig.WhiteItemsCost = Config.Bind<uint>(
                "Settings",
                "WhiteItemsCost",
                1,
                "Cost of white items"
            );

            PluginConfig.GreenItemsCost = Config.Bind<uint>(
                "Settings",
                "GreenItemsCost",
                2,
                "Cost of green items"
            );

            PluginConfig.RedItemsCost = Config.Bind<uint>(
                "Settings",
                "RedItemsCost",
                4,
                "Cost of red items"
            );

            PluginConfig.BossItemsCost = Config.Bind<uint>(
                "Settings",
                "BossItemsCost",
                2,
                "Cost of boss items"
            );

            PluginConfig.BlueItemsCost = Config.Bind<uint>(
                "Settings",
                "BlueItemsCost",
                0,
                "Cost of blue items"
            );
        }
    }
}