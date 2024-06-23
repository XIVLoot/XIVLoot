namespace FFXIV_RaidLootAPI.DTO;

public class ParamUpdateDTO
{
    // Returns cost values of BiS Vs. current set
    public int BOOL_LOCK_PLAYERS { get; set; }
    public int BOOL_LOCK_IF_NOT_CONTESTED { get; set; }
    public int RESET_TIME_IN_WEEK { get; set; }
    public int BOOL_FOR_1_FIGHT { get; set; }
    public int INT_NUMBER_OF_PIECES_UNTIL_LOCK { get; set; }
    public int LOCK_IF_TOME_AUGMENT { get; set; }
    public int BOOL_IF_ROLE_CHANGES_NUMBER_PIECES { get; set; }
    public int DPS_NUMBER { get; set; }
    public int TANK_NUMBER { get; set; }
    public int HEALER_NUMBER { get; set; }

    public Dictionary<string, int> DTOAsDict(){
        return new Dictionary<string, int> {
            { "BOOL_LOCK_PLAYERS", BOOL_LOCK_PLAYERS }, 
            { "BOOL_LOCK_IF_NOT_CONTESTED", BOOL_LOCK_IF_NOT_CONTESTED }, 
            { "RESET_TIME_IN_WEEK", RESET_TIME_IN_WEEK }, 
            { "BOOL_FOR_1_FIGHT", BOOL_FOR_1_FIGHT }, 
            { "INT_NUMBER_OF_PIECES_UNTIL_LOCK", INT_NUMBER_OF_PIECES_UNTIL_LOCK }, 
            { "LOCK_IF_TOME_AUGMENT", LOCK_IF_TOME_AUGMENT }, 
            { "BOOL_IF_ROLE_CHANGES_NUMBER_PIECES", BOOL_IF_ROLE_CHANGES_NUMBER_PIECES }, 
            { "DPS_NUMBER", DPS_NUMBER }, 
            { "TANK_NUMBER", TANK_NUMBER }, 
            { "HEALER_NUMBER", HEALER_NUMBER } };
    }

}