using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace Mordrog
{
    public static class UsersHelper
    {
        public static CharacterMaster GetUserMaster(CharacterBody body)
        {
            foreach (var player in PlayerCharacterMasterController.instances.Select(p => p.master))
            {
                if (player.GetBody() == body)
                    return player;
            }

            return null;
        }

        public static Interactor GetUserInteractor(NetworkUser user)
        {
            if (!user || !user.master)
                return null;

            var body = user.master.GetBodyObject();

            return body ? body.GetComponent<InteractionDriver>().interactor : null;
        }

        public static IEnumerable<NetworkUser> GetAllUsers()
        {
            return NetworkUser.readOnlyInstancesList;
        }

        public static NetworkUser GetUser(CharacterMaster userMaster)
        {
            return NetworkUser.readOnlyInstancesList.FirstOrDefault(u => u.master == userMaster);
        }

        public static NetworkUser GetUser(CharacterBody userBody)
        {
            return GetUser(GetUserMaster(userBody));
        }

        public static NetworkUser GetUser(Interactor userInteractor)
        {
            var body = userInteractor?.GetComponent<CharacterBody>();

            return GetUser(body);
        }

        public static NetworkUser GetUser(PingerController userPinger)
        {
            var player = userPinger?.GetComponent<CharacterMaster>(); 

            return GetUser(player);
        }
    }
}
