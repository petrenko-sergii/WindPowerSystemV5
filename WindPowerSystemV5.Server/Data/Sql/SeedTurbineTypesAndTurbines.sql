
--- SEED TurbineTypes and Turbines

INSERT INTO [WIND-POWER-SYSTEM].[dbo].[TurbineTypes] VALUES ('Vestas', 'V39/600', 600);
INSERT INTO [WIND-POWER-SYSTEM].[dbo].[TurbineTypes] VALUES ('Siemens Gamesa', 'SG 2.1-114', 1140);
INSERT INTO [WIND-POWER-SYSTEM].[dbo].[TurbineTypes] VALUES ('Nordex', 'N43', 800);
INSERT INTO [WIND-POWER-SYSTEM].[dbo].[TurbineTypes] VALUES ('Enercon', 'E-44', 900);

INSERT INTO [WIND-POWER-SYSTEM].[dbo].[Turbines] VALUES ('V52/850/2014-dk/kol-863', 850, 1);
INSERT INTO [WIND-POWER-SYSTEM].[dbo].[Turbines] VALUES ('SG2.1-114/2013-dk/kol-605', 1140, 2);
INSERT INTO [WIND-POWER-SYSTEM].[dbo].[Turbines] VALUES ('N43/2011-dk/kol-536', 800, 3);
INSERT INTO [WIND-POWER-SYSTEM].[dbo].[Turbines] VALUES ('E-44/2016-dk/kol-221', 900, 4);

select * from  [dbo].[TurbineTypes]
select * from  [dbo].[Turbines]