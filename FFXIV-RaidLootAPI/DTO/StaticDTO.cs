using System.ComponentModel.DataAnnotations;
using FFXIV_RaidLootAPI.Models;

namespace FFXIV_RaidLootAPI.DTO;

public class StaticDTO
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string UUID { get; set; } = string.Empty;
    
    public List<Players> Players { get; set; } = new List<Players>();
}