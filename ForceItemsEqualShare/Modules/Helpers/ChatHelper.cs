namespace Mordrog
{
    public static class ChatHelper
    {
        private const string RedColor = "ff0000";
        private const string GreenColor = "32cd32";


        public static void PlayerHaveTooManyItems(string userName, string userWithLeastItemsName)
        {
            var message = $"<color=#{RedColor}>{userName} has too many items!</color> <color=#{GreenColor}>Share some with {userWithLeastItemsName}!</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }
    }
}
