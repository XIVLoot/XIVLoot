using System.ComponentModel.DataAnnotations;
using FFXIV_RaidLootAPI.Models;
namespace FFXIV_RaidLootAPI.DTO;

public class PlayerDTO

{
    [Required]
    public int Id {get;set;}
    public bool UseBis {get;set;}
    public GearType GearToChange {get;set;}
    public Turn turn {get;set;}
    public int NewGearId {get;set;}
    public string NewEtro {get;set;} = string.Empty;
    public string NewName {get;set;} = string.Empty;
    public Job NewJob {get;set;}
    public bool CheckLockPlayer {get;set;}


}