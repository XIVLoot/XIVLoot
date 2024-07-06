using Microsoft.AspNetCore.Identity;

namespace FFXIV_RaidLootAPI.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string user_saved_static {get;set;} = string.Empty;
        public string user_displayed_name {get;set;} = string.Empty;
    }
}