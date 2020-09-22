using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace Mordrog
{
    public static class PlayersHelper
    {
        public static CharacterMaster GetPlayer(CharacterBody body)
        {
            foreach (var player in PlayerCharacterMasterController.instances.Select(p => p.master))
            {
                if (player.GetBody() == body)
                    return player;
            }

            return null;
        }

        public static IEnumerable<CharacterMaster> GetAllPlayers()
        {
            return PlayerCharacterMasterController.instances.Select(p => p.master);
        }
    }
}
