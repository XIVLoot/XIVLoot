

using System.ComponentModel.DataAnnotations;

namespace FFXIV_RaidLootAPI.User{
    public class Users
    { 
        public string user_discord_id {get;set;} = "1";
        public string user_saved_static {get;set;} = string.Empty;
        public int Id {get;set;}
    }
}