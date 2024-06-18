

using System.ComponentModel.DataAnnotations;

namespace FFXIV_RaidLootAPI.User{
    public class Users
    {   [Key]
        public string user_discord_id {get;set;} = string.Empty;
        public string user_saved_static {get;set;} = string.Empty;
    }
}