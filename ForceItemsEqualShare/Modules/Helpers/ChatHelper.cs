using UnityEngine;

namespace Mordrog
{
    public static class ChatHelper
    {
        private const string RedColor = "ff0000";
        private const string GreenColor = "32cd32";
        private const string SilverColor = "c0c0c0";


        public static void PlayerHasTooManyItems(string userName)
        {
            var message = $"<color=#{RedColor}>{userName} has too many items!</color> <color=#{GreenColor}>Share by</color> <color=#{SilverColor}>ping + E button.</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }

        public static void PlayerBoostedWithItem(string userName, string pickupToken, Color itemColor)
        {
            var message = $"<style=cEvent><noparse>{userName}</noparse> boosted with <color=#{ColorUtility.ToHtmlStringRGB(itemColor)}>{RoR2.Language.GetString(pickupToken) ?? "???"}</color>";
            RoR2.Chat.SendBroadcastChat(new RoR2.Chat.SimpleChatMessage { baseToken = message });
        }
    }
}
