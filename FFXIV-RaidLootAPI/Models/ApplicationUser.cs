using Microsoft.AspNetCore.Identity;

namespace FFXIV_RaidLootAPI.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string user_saved_static {get;set;} = string.Empty;
        public string user_displayed_name {get;set;} = string.Empty;
        public string user_claimed_playerId {get;set;} = string.Empty;

        public bool UserClaimedPlayer(string playerId){
            foreach(string id in user_claimed_playerId.Split(";")){
                if (id == playerId)
                    return true;
            }
            return false;
        }

        public void removePlayerClaim(string playerId){
            List<string> uuidList = user_claimed_playerId.Split(';').ToList();
            uuidList.Remove(playerId);
            user_claimed_playerId = String.Join(";", uuidList);
        }

        public List<string> getAllClaimedPlayerId(){
            return user_claimed_playerId.Split(";").ToList();
        }
    }
}