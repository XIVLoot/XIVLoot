export const gearAcquisitionToolTip = `This section shows the history of Gear Acquistion. To remove erroneous entries, click on the corresponding gear coffer. 
                                       This will only track Savage Raid gear and Augmented Tome items.`;

export const pgsToolTip = `This is an experimental feature, we are breaking down loot based on a jobs rDPS ranking (via FFLogs), a players individual iLVL, and how well a job performs in a buff window (Dynamically checked for your group). 
                           This will possibly give you a better idea on who should receive items throughout the tier. This data is displayed from left to right with jobs in "gold" being the best bet for item upgrades and green being the 
                           least beneficial for an upgrade.`;

export const pgsSettingToolTipA = `The value of this parameter is related to the individual contribution of the player. The higher this parameter's value is the more the PGS will value giving gear to players that have a high rDPS value.
                                   This will for example favour jobs like blackmage/samurai/viper that offer high rDPS regardless of buff padding.`;

export const pgsSettingToolTipB = `The value of this parameter is related to the average item level distribution amounts the group. The higher this parameter's value is the more PGS will value giving gear to players whose item level is below the group's average item level. In other words the gear will be distributed evenly among the players regardless of the job.`;

export const pgsSettingToolTipC = `The value of this parameter is related to the player's contribution to buff padding. The higher this parameter's value is the more PGS will value giving gear to player who can add a lot of damage in burst. Note that this also takes into account how many raid buffs the static's teamcomp has.
                                   This will for example favour jobs like samurai/ninja that offer a lot of burst.`;

export const lockLogicToolTip = `Some Statics may employ a loot distribution system that locks out  members each week after they receive gear. This lockout will update automatically based on the settings you use below.`;

export const lockOutOfGearEvenIfNotContestedToolTip = `This option lets you lock a player out of gear even if the gear the player acquired was not contested by anyone else. Otherwise a player will only
                                                       be locked out of gear if it acquired gear someone else was contesting.`;

export const lockPerFightToolTip = `This option lets you lock a player as a per fight basis. So if a player receives gear from the first turn they will be (possibly) locked from only the first turn.
                                    Otherwise a player is locked from all fights as soon as it is locked from one.`;

export const lockPlayerForAugmentToolTip = `This option lets you lock a player for receiving augment items. If this option is turned on a player that received a gear augment item
                                     will be locked from the fight just like if the player had received raid gear.`;

export const pieceUntilLockToolTip = `THIS FEATURE IS CURRENTLY DISABLED.This allows you to configure how many gear piece a player can receive before being locked.`;

export const numberWeekResetToolTip = `This allows you to configure for how many weeks a player is locked once it is first locked. Note that the lock starts at the next loot pool reset (every tuesday 4 am EST) and not as soon as gear is acquired.`

export const etroToolTip = `To import either a current or BiS gearset click on the 'Import gear' button. If you import a BiS gearset it will be saved to this character and you can then access it on its original website using the button to the left of 'Import gear'.`;

export const pgsOnPlayerToolTip = `This is the current PGS value of the player. The colored indicator refers to the color grouping priority (gold, purple, blue and green in this order). The player(s) with the smallest PGS
                                    value will be recommended to receive gear by the PGS analysis.`;

export const lockOnPlayerToolTip = `This section shows all fights the player is currently locked from with the date the lock will be lifted. Click on a fight to remove the lock if needed.`;

export const gearSelectionToolTip = `This section allows you to see and edit your current and best in slot gear set. The section is seperated in two parts : one for equipement pieces (aka left side) and one for accessories (aka right side).
                                     In both those sections, the left column represents your current gearset while the right column represents the best in slot gearset of the player.`;
                                    
export const claimPlayerToolTip = `Claim this player to be the sole user with the ability to modify it. You will also be able to see all your claimed players on the homescreen. You must be logged in to claim a player.`;

export const unclaimPlayerToolTip = `Un-claiming this player will let anyone with the link to this static modify it. This will also remove this character from your homescreen.`;

export const alreadyClaimedToolTip = `Someone else has already claimed the player. You therefore cannot modify it. If you think this is an error please reach out.`;

export const UseBookForGearAcqToolTip = `By Default any gear obtained will be treated as an item drop from the respective fight. Use this option to indicate that an item was obtained via a book. Please toggle this option off after the item has been added.`;

export const HomeClaimPlayerToolTip = `Here you will see all of your claimed players. Claimed players are players from static that only you have the ability to modify. In a way you own these players. To claim a player create a static and select a player to edit, you will then see a "Claim Player" option above the edit window if this player has not been claimed yet. To unclaim a player, navigate to a previously claimed player and above the edit window there will be a "Unclaim player" option which you click to unclaim the player.`;

export const UseBisToolTip = `To import the etro.gg or xivgear.app gearset as your current gear set uncheck this option. To import it as your current BiS check this option. If you import as your BiS the link will be saved with this character
                              and you will be able to view the gearset on its original website by clicking the button next to the 'Import' button on the character's view page.`;

export const GearBreakdownToolTip = `Within this section, you can find a breakdown of all Raid items required by members of the static. Simply select the Boss on the left side to see a color coordinated view into who needs what item for their BIS build. Green indicates a player is not locked out of getting loot, while red shows players who are locked out based on the loot rules your static follows.`;

export const FreePlayerToolTip = `Click this button to free a player from all claim it has received. This option is only available to the static's creator.`;

export const ClaimStaticToolTip = `Please have the Static leader claim this group. Only members with a claimed player can become the leader of the static. A static leader is given the right to unclaim any previously claimed players. Additional features will come.`;

export const UnclaimStaticToolTip = `If the static leader is incorrect, please have them un-claim the static. For additional assistance with this, please contact us via email at Support@XIVLoot.com`;


export const CheckWeekDoneToolTip = `Click this button to mark the week as done.`;

export const CheckWeekNotDoneToolTip = `Click this button to mark the week as not done.`;

export const TotalTomestonesToolTip = `This is the total number of Tomestones needed to complete the tome plan.`;

export const StartingTomeToolTip = `Input your starting amount of tomestones here.`;

export const DeleteWeekToolTip = `Click this button to delete the week. A week cannot be deleted if it is needed to complete the tome plan.`;

export const AddWeekToolTip = `Click this button to add a week at the end.`;

export const AddWeekStartToolTip = `Click this button to add a week at the start.`;




