x = "INSERT [dbo].[Gears] ([Id], [Name], [GearLevel], [GearType], [GearStage], [GearCategory], [GearWeaponCategory], [IconPath], [EtroGearId], [Tier]) VALUES (1, N'No Equipment', 0, 0, 0, 0, 0, N'0', 0, -1)"

GearToGen = [(740, "Normal raid", 6), 
             (740, "Crafted", 1), 
             (750, "Tome gear", 2), 
             (760,"Augmented tome", 3), 
             (760, "Raid", 4), 
             (745, "Extreme", 5)]
tier = 1
genSQLString = """
USE [LootManagementDB]
GO
SET IDENTITY_INSERT [dbo].[Gears] ON 

INSERT [dbo].[Gears] ([Id], [Name], [GearLevel], [GearType], [GearStage], [GearCategory], [GearWeaponCategory], [IconPath], [EtroGearId], [Tier]) VALUES (1, N'No Equipment', 0, 0, 0, 0, 0, N'0', 0, -1)
"""

GearCategory = [1,2,3,4,5,6,7,8]
#Fending = 1,
#Maiming = 2,
#Striking = 3,
#Scouting = 4,
#Aiming = 5,
#Casting = 6,
#Healing = 7,
#Slaying = 8,
#Weapon = 9
GearStage = [1,2,3,4]
#Preparation = 1,
#Tomes = 2,
#Upgraded_Tomes = 3,
#Raid = 4
GearType=[2,3,4,5,6,7,8,9,10,11]
#Weapon = 1,
#Head = 2,
#Body = 3,
#Hands = 4,
#Legs = 5,
#Feet = 6,
#Earrings = 7,
#Necklace = 8,
#Bracelets = 9,
#RightRing = 10,
#LeftRing = 11

id = 1000

for gearInfo in GearToGen:
    name = gearInfo[1]
    iLevel = gearInfo[0]
    stage=gearInfo[2]
    for type in GearType:  
        if name == "Extreme" and type in [2,3,4,5,6,7,8,9,10,11]:# only weapon ,2,3,4,5,6]: # Only acc and weapon
            continue
        for cat in GearCategory:
            genSQLString+= (f"INSERT [dbo].[Gears] ([Id], [Name], [GearLevel], [GearType], [GearStage], [GearCategory], [GearWeaponCategory], [IconPath], [EtroGearId], [Tier]) VALUES ({id}, N'{name}', {iLevel}, {type}, {stage}, {cat}, N'0', N'0', 0, {tier})\n")
            id+=1

    # Generating weapon
    for i in range(1,22):
        if name == "Raid":
            genSQLString+= f"INSERT [dbo].[Gears] ([Id], [Name], [GearLevel], [GearType], [GearStage], [GearCategory], [GearWeaponCategory], [IconPath], [EtroGearId], [Tier]) VALUES ({id}, N'{name}', {iLevel+5}, 1, {stage}, 9, N'{i}', N'0', 0, {tier})\n"
            id+=1
        else :
            genSQLString+= f"INSERT [dbo].[Gears] ([Id], [Name], [GearLevel], [GearType], [GearStage], [GearCategory], [GearWeaponCategory], [IconPath], [EtroGearId], [Tier]) VALUES ({id}, N'{name}', {iLevel}, 1, {stage}, 9, N'{i}', N'0', 0, {tier})\n"
            id+=1

        # Open a file in write mode
with open('output.sql', 'w') as file:
    # Write the contents of genSQLString to the file
    file.write(genSQLString)

