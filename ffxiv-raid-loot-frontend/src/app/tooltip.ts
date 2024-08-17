export const gearAcquisitionToolTip = `This section shows the history of gear acquisition. To remove erroneous entries, click on the corresponding gear coffer. 
                                       This will only track Savage Raid gear and Augmented Tome items.`;

export const pgsToolTip = `This is an experimental feature. It assigns a priority for gear distribution based on each player's iLVL and their job's rDPS rank (via FFLogs) and performance under buff windows (dynamically checked for your group's composition). Given these parameters, each player has a PGS score represented by a colour, from highest priority to lowest: gold, purple, blue, green. This will give the group an idea of which players should be prioritised for upgrades throughout the tier.`;

export const pgsSettingToolTipA = `This parameter is related to a player's individual contribution. Higher values will prioritise giving gear to players that have high rDPS.
                                   This will, for example, favour jobs such as Black Mage/Samurai/Viper that offer high rDPS regardless of buff padding.`;

export const pgsSettingToolTipB = `This parameter is related to the average item level distribution among the group. Higher values will prioritise giving gear to players whose item level is below the group's average item level. In other words, the gear will be distributed evenly among the players regardless of job.`;

export const pgsSettingToolTipC = `This parameter is related to a player's contribution to buff padding. Higher values will prioritise giving gear to players that can contribute a lot of damage in burst. Note that this also takes into account how many raid buffs the static's composition has.
                                   This will, for example, favour jobs such as Samurai/Ninja that offer a lot of burst.`;

export const lockLogicToolTip = `Your static may employ a loot distribution system that locks out members from acquiring loot again for a certain period after receiving gear. You can customise this setting to your liking below.`;

export const lockOutOfGearEvenIfNotContestedToolTip = `A player is locked out of gear even if the item they acquired was not contested by anyone else. Otherwise, a player will only
                                                       be locked out of gear they acquired an item that someone else was contesting.`;

export const lockPerFightToolTip = `Lock a player on a per fight basis.  Otherwise, a player is locked from all fights as soon as they are locked from one.`;

export const lockPlayerForAugmentToolTip = `If this option is enabled, a player that receives a gear augment item will be locked from receiving more loot just as if they had received raid gear.`;

export const pieceUntilLockToolTip = `THIS FEATURE IS CURRENTLY DISABLED. Specify how many items a player can receive before being locked.`;

export const numberWeekResetToolTip = `Specify how many weeks a player is locked out of gear for once they are first locked. Note that the lock starts at the next loot pool reset (every Tuesday, 4 a.m. EST), and not as soon as gear is acquired.`

export const etroToolTip = `Import either a current gearset or a BiS gearset from etro.gg or xivgear.app. If imported as a BiS, the link will be saved and you will be able to view the gearset on its original website by clicking the button next to the 'Import' button.`;

export const pgsOnPlayerToolTip = `This is the player's current PGS value. The colour indicates priority (from highest to lowest: gold, purple, blue and green). The player(s) with the smallest PGS value will be prioritised to receive gear by the PGS analysis.`;

export const lockOnPlayerToolTip = `This section shows all fights the player is currently locked from and the date the lock will be lifted. Click on a fight to remove the lock if needed.`;
                                    
export const claimPlayerToolTip = `Claim this player to become the sole user with the ability to modify it. You will be able to see all your claimed players on the home screen. You must be logged in to claim a player.`;

export const unclaimPlayerToolTip = `Unclaiming this player will enable anyone with the link to this static to modify it. This will also remove this character from your home screen.`;

export const alreadyClaimedToolTip = `Someone else has already claimed this player. You therefore cannot modify it. If you think this is an error, please reach out.`;

export const UseBookForGearAcqToolTip = `By default, any gear obtained will be treated as an item drop from the respective fight. Enable this option to indicate that an item was obtained using books. Please toggle this option off after the item has been added.`;

export const HomeClaimPlayerToolTip = `Claimed players in a static can only be modified by you. To claim or unclaim a player, navigate to your static's page, select the desired player and click the corresponding button.`;

export const UseBisToolTip = `If this option is unchecked, the gearset will be imported as your current gear. If it is checked, it will be imported as your BiS. If imported as BiS, the link will be saved and you will be able to view the gearset on its original website by clicking the button next to the 'Import' button on the character's view page.`;

export const GearBreakdownToolTip = `This section contains a breakdown of all items required by members of the static. Select the fight on the left side to see a colour-coordinated view of who needs what item for their BiS. Green indicates a player is not locked out of getting loot, while red means they are locked out based on the loot rules set by your static. An orange player indicates an 'alt' account that is not locked out of gear.`;

export const FreePlayerToolTip = `Click this button to free a player from their claim. This option is only available to the static's creator.`;

export const ClaimStaticToolTip = `Please have the static leader claim this group. Only users with a claimed player can become a static leader. A static leader is given the right to unclaim any previously claimed players. Additional features will come.`;

export const UnclaimStaticToolTip = `If the static leader is incorrect, please have them unclaim the static. For additional assistance with this, please reach out.`;


export const CheckWeekDoneToolTip = `Click to mark the week as done.`;

export const CheckWeekNotDoneToolTip = `Click to mark the week as not done.`;

export const TotalTomestonesToolTip = `This is the total number of tomestones needed to complete the tome plan.`;

export const StartingTomeToolTip = `Input your starting amount of tomestones here.`;

export const DeleteWeekToolTip = `Click to delete the week. A week cannot be deleted if it is needed to complete the tome plan.`;

export const AddWeekToolTip = `Click to add a week at the end.`;

export const AddWeekStartToolTip = `Click to add a week at the start.`;

export const AddGearToPlanToolTip = `Click to add gear to the tome plan.`;

export const AddGearLockToolTip = `You cannot add gear this week as you lack tomestones. Add a week at the start to have more tomestones by this point.`;

export const SurplusTomestonesToolTip = `Amount of tomestones you can spend this week without changing the planned schedule. Note that this amount is cumulative, so if you spend tomestones of this surplus make sure to include it in the 'Offset Tome Amount' box so it updates the surplus accordingly.`;

export const TomestoneOffsetToolTip = `Input the amount of tomestones you spent outside of the plan.`;

export const NeededGearToolTip = `This section shows the tome gear that is needed to complete this character's BiS gearset.`;

export const EOWTomestonesToolTip = `This is the amount of tomestones you will have by the end of this week.`;

export const GearCostToolTip = `Body - 825&#13;Legs - 825\nWeapon - 500\nHead - 495\nHands - 495\nFeet - 495\nEarrings - 375\nNecklace - 375\nBracelet - 375\nRing - 495\n`;


export const addNewPlayerToolTip = `Add a new player to the static. New players are tagged as being 'alt' players and they will appear differently in the 'Raid Loot Breakdown'.`;





// This tooltip is deleted but do not remove it.
export const gearSelectionToolTip = `This section allows you to see and edit your current and best in slot gear set. The section is seperated in two parts : one for equipement pieces (aka left side) and one for accessories (aka right side).
                                     In both those sections, the left column represents your current gearset while the right column represents the best in slot gearset of the player.`;
  




