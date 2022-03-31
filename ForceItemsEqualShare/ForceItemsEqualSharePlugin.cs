using BepInEx;

namespace Mordrog
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class ForceItemsEqualSharePlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.3.1";
        public const string ModName = "ForceItemsEqualShare";
        public const string ModGuid = "com.Mordrog.ForceItemsEqualShare";

        public ForceItemsEqualSharePlugin()
        {
            InitConfig();
        }

        public void Awake()
        {
            base.gameObject.AddComponent<PickupItemsPermissionController>();
        }

        private void InitConfig()
        {
            PluginConfig.HowToHandleItemDisproportion = Config.Bind<HowToHandleItemDisproportion>(
                "Settings",
                "HowToHandleItemDisproportion",
                HowToHandleItemDisproportion.GiveRandomItemToLowestCostsPlayer,
                "Way to handle items disproportion between player with loweset and biggest item costs"
            );

            PluginConfig.ScaleItemsCostsDifference = Config.Bind<float>(
                "Settings",
                "ScaleItemsCostsDifference",
                0.3f,
                "The scale items costs difference between the player picking item and the player with the lowest number of items costs.\n" +
                "Too many items formula: Min(Max(PlayersItems * ScaleItemsDifference, MinItemsDifference), MaxItemsDifference) <= PlayersItems - LeastPlayerItems\n" +
                "Have to be between 0 and 1"
            );

            PluginConfig.MinItemsCostsDifference = Config.Bind<uint>(
                "Settings",
                "MinItemsCostsDifference",
                5,
                "The min items costs difference between the player trying to pick an item and the player with the lowest number of items costs.\n" +
                "Have to be equal or lesser then MaxItemsDifference"
            );

            PluginConfig.MaxItemsCostsDifference = Config.Bind<uint>(
                "Settings",
                "MaxItemsCostsDifference",
                15,
                "The max items costs difference between the player trying to pick an item and the player with the lowest number of items costs.\n" +
                "Have to be equal or bigger then MinItemsDifference"
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