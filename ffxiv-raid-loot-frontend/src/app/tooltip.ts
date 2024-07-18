export const gearAcquisitionToolTip = `This section shows the history of gear acquisition. Click on a gear coffer to remove the event if it is erroneous.
                                       This will only show raid gear or augment items and ignores preperation type gears (crafted, etc.) and non-augment tome gear.`;
export const pgsToolTip = `This section shows the suggested priority of which player should receive gear according to your own settings. 
                           The different grouping of colors is based on the PGS of each player. Players in the left most column have the highest
                           gear receiving priority while the players in the right most column have the lowest priority. There are four possible grouping colors :
                           gold, purple, blue and green. The prority is decreasing in that order (with gold being the highest and green being the lowest).`;
export const pgsSettingToolTipA = `The value of this parameter is related to the individual contribution of the player. The higher this parameter's value is the more the PGS will value giving gear to players that have a high rDPS value.
                                   This will for example favour jobs like blackmage/samurai/viper that offer high rDPS regardless of buff padding.`;
export const pgsSettingToolTipB = `The value of this parameter is related to the average item level distribution amounts the group. The higher this parameter's value is the more PGS will value giving gear to players whos item level is below the group's average item level.`;
export const pgsSettingToolTipC = `The value of this parameter is related to the player's contribution to buff padding. The higher this parameter's value is the more PGS will value giving gear to player who can add a lot of damage in burst. Note that this also takes into account how many raid buffs the static's teamcomp has.
                                   This will for example favour jobs like samurai/ninja that offer a lot of burst.`;

export const lockLogicToolTip = `The locking system allows you to control who can and cannot receive gear. It allows you to customize the locking system according to your needs.
                                 A player who is locked out of gear will see the corresponding fight be darker with a message displaying at what date they will be able to receive gear again.
                                 In the case where a player who cannot receive gear still wants gear a message will open asking to confirm the gear acquisition. Note that a locked player's unlocking date
                                 will update itself if the player receives`;
export const lockOutOfGearEvenIfNotContestedToolTip = `This option lets you lock a player out of gear even if the gear the player acquired was not contested by anyone else. Otherwise a player will only
                                                       be locked out of gear if it acquired gear someone else was contesting.`;
export const lockPerFightToolTip = `This option lets you lock a player as a per fight basis. So if a player receives gear from the first turn they will be (possibly) locked from only the first turn.
                                    Otherwise a player is locked from all fights as soon as it is locked from once.`;
export const lockPlayerForAugmentToolTip = `This option lets you lock a player for receiving augment items. If this option is turned on a player that received a gear augment item
                                     will be locked from the fight just like if the player had received raid gear.`;
export const pieceUntilLockToolTip = `This allows you to configure how many gear piece a player can receive before being locked.`;

export const numberWeekResetToolTip = `This allows you to configure for how many weeks a player is locked once it is first locked.`

export const etroToolTip = `Put the link (or only the UUID of the gearset) of any etro.gg gearset and click the "Import" button to be prompted to import it as your new best in slot. Click the "Etro.gg" button to view the 
                            gearset on etro.gg. Note that it only saves the gearset's UUID (a unique identifier) so you will not find the whole etro.gg link when you come back to to this website.`;
export const pgsOnPlayerToolTip = `This is the current PGS value of the player. The colored indicator refers to the color grouping priority (gold, purple, blue and green in this order). The player(s) with the smallest PGS
                                    value will be recommended to receive gear by the PGS analysis.`;
export const lockOnPlayerToolTip = `This section shows all fights the player is currently locked from with the date the lock will be lifted. Click on a fight to remove the lock if needed.`;
export const gearSelectionToolTip = `This section allows you to see and edit your current and best in slot gear set. The section is seperated in two parts : one for gear piece (aka left side) and one for accessories (aka right side).
                                     In both those sections, the left column represents your current gearset while the right column represents the best in slot gearset of the player.`;
                                    
export const claimPlayerToolTip = `Claim this player to be the sole user with the ability to modify it. You will also be able to see all your claimed players on the homescreen. You must be logged in to claim a player.`;
export const unclaimPlayerToolTip = `Unclaiming this player will let anyone else modify it. You will loose the ability to see the player on the homescreen.`;
export const alreadyClaimedToolTip = `Someone else has already claimed the player. You therefore cannot modify it. If you think this is an error please reach out.`;

export const UseBookForGearAcqToolTip = `This options allows to specify to the program if any gear acquisitioned was through books. If a gear is gained through book (rather than through raid) the player will not be locked (if such would have been the case) and the gear acquisition
                                         history will display that the specific gear coffer (or augment item) was obtained through the use of books.`;
export const HomeClaimPlayerToolTip = `Here you will see all of your claimed players. Claimed players are players from static that only you have the ability to modify. In a way you own these players. To claim a player create a static and select a player to edit, you will then see a "Claim Player" option above the edit window if this player has not been claimed yet. To unclaim a player, navigate to a previously claimed player and above the edit window there will be a "Unclaim player" option which you click to unclaim the player.`;