TRUNCATE TABLE dbo.Gears;

INSERT INTO dbo.Gears VALUES 
('CraftedWeapon', 80, 1, 1, 1, 10, 'TestIcon.icn'),
('CraftedHead', 80, 2, 1,  1,0, 'TestIcon.icn'),
('CraftedBody', 80, 3, 1,  1,0, 'TestIcon.icn'),
('CraftedHands', 80, 4, 1, 1, 0, 'TestIcon.icn'),
('CraftedLegs', 80, 5, 1,  1,0, 'TestIcon.icn'),
('CraftedFeet', 80, 6, 1,  1,0, 'TestIcon.icn'),
('CraftedEarrings', 80, 7, 1, 1, 0, 'TestIcon.icn'),
('CraftedNecklace', 80, 8, 1, 1, 0, 'TestIcon.icn'),
('CraftedBracelets', 80, 9, 1, 1, 0, 'TestIcon.icn'),
('CraftedRightRing', 80, 10, 1, 1, 0, 'TestIcon.icn'),
('CraftedLeftRing', 80, 11,  1,1, 0, 'TestIcon.icn'),

('AugmentedTomeWeapon', 150, 1, 1, 3, 10, 'TestIcon.icn'),
('AugmentedTomeHead', 150, 2, 3, 1, 0, 'TestIcon.icn'),
('AugmentedTomeBody', 150, 3, 3, 1, 0, 'TestIcon.icn'),
('AugmentedTomeHands', 150, 4, 3, 1, 0, 'TestIcon.icn'),
('AugmentedTomeLegs', 150, 5, 3,  1,0, 'TestIcon.icn'),
('AugmentedTomeFeet', 150, 6, 3,  1,0, 'TestIcon.icn'),
('AugmentedTomeEarrings', 150, 7, 3, 1, 0, 'TestIcon.icn'),
('AugmentedTomeNecklace', 150, 8, 3,  1,0, 'TestIcon.icn'),
('AugmentedTomeBracelets', 150, 9, 3,  1,0, 'TestIcon.icn'),
('AugmentedTomeRightRing', 150, 10, 3,  1,0, 'TestIcon.icn'),
('AugmentedTomeLeftRing', 150, 11, 3, 1,0, 'TestIcon.icn'),

('TomeWeapon', 100, 1, 2,  1,10, 'TestIcon.icn'),
('TomeHead', 100, 2, 2,  1,0, 'TestIcon.icn'),
('TomeBody', 100, 3, 2,  1,0, 'TestIcon.icn'),
('TomeHands', 100, 4, 2,  1,0, 'TestIcon.icn'),
('TomeLegs', 100, 5, 2,  1,0, 'TestIcon.icn'),
('TomeFeet', 100, 6, 2,  1,0, 'TestIcon.icn'),
('TomeEarrings', 100, 7, 2,  1,0, 'TestIcon.icn'),
('TomeNecklace', 100, 8, 2,  1,0, 'TestIcon.icn'),
('TomeBracelets', 100, 9, 2,  1,0, 'TestIcon.icn'),
('TomeRightRing', 100, 10, 2,  1,0, 'TestIcon.icn'),
('TomeLeftRing', 100, 11, 2,  1,0, 'TestIcon.icn'),

('RaidWeapon', 150, 1, 4,  1,10, 'TestIcon.icn'),
('RaidHead', 150, 2, 4,  1,0, 'TestIcon.icn'),
('RaidBody', 150, 3, 4,  1,0, 'TestIcon.icn'),
('RaidHands', 150, 4, 4,  1,0, 'TestIcon.icn'),
('RaidLegs', 150, 5, 4,  1,0, 'TestIcon.icn'),
('RaidFeet', 150, 6, 4,  1,0, 'TestIcon.icn'),
('RaidEarrings', 150, 7, 4,  1,0, 'TestIcon.icn'),
('RaidNecklace', 150, 8, 4,  1,0, 'TestIcon.icn'),
('RaidBracelets', 150, 9, 4,  1,0, 'TestIcon.icn'),
('RaidRightRing', 150, 10, 4,  1,0, 'TestIcon.icn'),
('RaidLeftRing', 150, 11, 4,  1,0, 'TestIcon.icn');


SELECT * FROM dbo.Gears;