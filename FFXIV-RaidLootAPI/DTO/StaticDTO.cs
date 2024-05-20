using System.ComponentModel.DataAnnotations;

namespace FFXIV_RaidLootAPI.DTO;

public class StaticDTO
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string UUID { get; set; } = string.Empty;
}