﻿using System;
using System.Collections.Generic;
using System.Linq;
using Constructor.Satellites;
using Constructor.Ships.Modification;
using Database.Legacy;
using Economy.ItemType;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameServices.Player;
using Maths;
using Session.Content;

namespace Constructor.Ships
{
    public static class ShipExtensions
    {
        public static bool IsSuitableSatelliteSize(this IShip ship, Satellite satellite)
        {
            if (satellite.SizeClass != SizeClass.Undefined)
                return ship.Model.SizeClass >= satellite.SizeClass;

            return ship.Model.ModelScale >= satellite.ModelScale * 2;
        }

        public static int Price(this IShip ship)
        {
            return ship.Model.Layout.CellCount * ship.Model.Layout.CellCount + ship.Model.SecondLayout.CellCount * ship.Model.SecondLayout.CellCount;
        }

        public static int Scraps(this IShip ship)
        {
            return ship.Model.Layout.CellCount / 5 + ship.Model.SecondLayout.CellCount / 3;
        }

        public static IShip RandomizeColor(this IShip ship, System.Random random)
        {
            ship.ColorScheme.Type = ShipColorScheme.SchemeType.Default;
            ship.ColorScheme.Hue = random.Percentage(50) ? random.NextFloat() * 0.2f : 1.0f - random.NextFloat() * 0.2f;
            ship.ColorScheme.Saturation = random.NextFloat() * 0.1f;
            return ship;
        }

        public static IShip CreateCopy(this IShip ship)
        {
            return new CommonShip(ship.Model, ship.Components)
            {
                Satellite_Left_1 = ship.Satellite_Left_1.CreateCopy(),
                Satellite_Right_1 = ship.Satellite_Right_1.CreateCopy(),
                Satellite_Left_2 = ship.Satellite_Left_2.CreateCopy(),
                Satellite_Right_2 = ship.Satellite_Right_2.CreateCopy(),
                Satellite_Left_3 = ship.Satellite_Left_3.CreateCopy(),
                Satellite_Right_3 = ship.Satellite_Right_3.CreateCopy(),
                Satellite_Left_4 = ship.Satellite_Left_4.CreateCopy(),
                Satellite_Right_4 = ship.Satellite_Right_4.CreateCopy(),
                Satellite_Left_5 = ship.Satellite_Left_5.CreateCopy(),
                Satellite_Right_5 = ship.Satellite_Right_5.CreateCopy(),
                Name = ship.Name
            };
        }

        public static void SetLevel(this IShip ship, int level)
        {
            ship.Experience = Maths.Experience.FromLevel(level);
        }

        public static IShipSpecification BuildSpecAndApplySkills(this IShip ship, PlayerSkills skills, ShipSettings settings)
        {
            var builder = ship.CreateBuilder();

            builder.Bonuses.DamageMultiplier *= skills.AttackMultiplier;
            builder.Bonuses.ArmorPointsMultiplier *= skills.DefenseMultiplier;
            builder.Bonuses.ShieldPointsMultiplier *= skills.DefenseMultiplier + skills.ShieldStrengthBonus;
            builder.Bonuses.ShieldRechargeMultiplier *= skills.ShieldRechargeMultiplier;

            builder.Bonuses.EnergyShieldPointsMultiplier *= skills.DefenseMultiplier + skills.EnergyShieldStrengthBonus;
            builder.Bonuses.EnergyShieldRechargeMultiplier *= skills.EnergyShieldRechargeMultiplier;

            builder.Bonuses.ExtraHeatResistance = skills.HeatResistance;
            builder.Bonuses.ExtraEnergyResistance = skills.EnergyResistance;
            builder.Bonuses.ExtraKineticResistance = skills.KineticResistance;
            builder.Bonuses.ExtraQuantumResistance = skills.QuantumResistance;

            builder.Bonuses.ExtraShieldHeatResistance = skills.ShieldHeatResistance;
            builder.Bonuses.ExtraShieldEnergyResistance = skills.ShieldEnergyResistance;
            builder.Bonuses.ExtraShieldKineticResistance = skills.ShieldKineticResistance;
            builder.Bonuses.ExtraShieldQuantumResistance = skills.ShieldQuantumResistance;

            builder.Bonuses.ExtraEnergyShieldHeatResistance = skills.EnergyShieldHeatResistance;
            builder.Bonuses.ExtraEnergyShieldEnergyResistance = skills.EnergyShieldEnergyResistance;
            builder.Bonuses.ExtraEnergyShieldKineticResistance = skills.EnergyShieldKineticResistance;
            builder.Bonuses.ExtraEnergyShieldQuantumResistance = skills.EnergyShieldQuantumResistance;

            return builder.Build(settings);
        }

        public static IShip FromShipData(IDatabase database, ShipData shipData)
        {
            var shipWrapper = database.GetShip(new ItemId<Ship>(shipData.Id));
            if (shipWrapper == null)
                return null;

            var shipModel = new ShipModel(shipWrapper);
            var factory = new ModificationFactory(database);
            shipModel.Modifications.Assign(shipData.Modifications.Modifications.Select(item => ShipModificationExtensions.Deserialize(item, factory)));
            shipModel.LayoutModifications.Deserialize(shipData.Modifications.Layout.ToArray());
            var components = shipData.Components.FromShipComponentsData(database);
            var ship = new CommonShip(shipModel, components);

            ship.Satellite_Left_1 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Left_1);
            ship.Satellite_Right_1 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Right_1);
            ship.Satellite_Left_2 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Left_2);
            ship.Satellite_Right_2 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Right_2);
            ship.Satellite_Left_3 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Left_3);
            ship.Satellite_Right_3 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Right_3);
            ship.Satellite_Left_4 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Left_4);
            ship.Satellite_Right_4 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Right_4);
            ship.Satellite_Left_5 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Left_5);
            ship.Satellite_Right_5 = SatelliteExtensions.FromSatelliteData(database, shipData.Satellite_Right_5);
            ship.Name = shipData.Name;
            ship.ColorScheme.Value = shipData.ColorScheme;
            ship.Experience = (long)shipData.Experience;
            ship.DataChanged = false;
            return ship;
        }

        public static ShipData ToShipData(this IShip ship)
        {
            return new ShipData
            {
                Id = ship.Id.Value,
                Name = ship.Name,
                ColorScheme = ship.ColorScheme.Value,
                Experience = (long)ship.Experience,
                Components = ship.Components.ToShipComponentsData(),
                Modifications = new ShipModificationsData
                {
                    Layout = ship.Model.LayoutModifications.Serialize(),
                    Modifications = ship.Model.Modifications.Select<IShipModification, long>(ShipModificationExtensions.Serialize)
                },
                Satellite_Left_1 = ship.Satellite_Left_1.ToSatelliteData(),
                Satellite_Right_1 = ship.Satellite_Right_1.ToSatelliteData(),
                Satellite_Left_2 = ship.Satellite_Left_2.ToSatelliteData(),
                Satellite_Right_2 = ship.Satellite_Right_2.ToSatelliteData(),
                Satellite_Left_3 = ship.Satellite_Left_3.ToSatelliteData(),
                Satellite_Right_3 = ship.Satellite_Right_3.ToSatelliteData(),
                Satellite_Left_4 = ship.Satellite_Left_4.ToSatelliteData(),
                Satellite_Right_4 = ship.Satellite_Right_4.ToSatelliteData(),
                Satellite_Left_5 = ship.Satellite_Left_5.ToSatelliteData(),
                Satellite_Right_5 = ship.Satellite_Right_5.ToSatelliteData(),
            };
        }

        #region Obsolete

        public static IShip FromShipInfoObsolete(IDatabase database, FleetData.ShipInfoObsolete shipInfo)
        {
            Ship baseShip;

            int shipId;
            if (int.TryParse(shipInfo.Id, out shipId))
                baseShip = database.GetShip(new ItemId<Ship>(shipId));
            else
                baseShip = database.GetShip(LegacyShipNames.GetId(shipInfo.Id));

            if (baseShip == null)
                return null;

            var shipModel = new ShipModel(baseShip);

            var factory = new ModificationFactory(database);
            shipModel.Modifications.Assign(shipInfo.Modifications.Select(item => ShipModificationExtensions.Deserialize(item, factory)));

            var components = shipInfo.Components.Select(item => ComponentExtensions.Deserialize(database, item));
            var ship = new CommonShip(shipModel, components);

            ship.Satellite_Left_1 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Left_1);
            ship.Satellite_Right_1 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Right_1);
            ship.Satellite_Left_2 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Left_2);
            ship.Satellite_Right_2 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Right_2);
            ship.Satellite_Left_3 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Left_3);
            ship.Satellite_Right_3 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Right_3);
            ship.Satellite_Left_4 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Left_4);
            ship.Satellite_Right_4 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Right_4);
            ship.Satellite_Left_5 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Left_5);
            ship.Satellite_Right_5 = SatelliteExtensions.FromSatelliteInfo(database, shipInfo.Satellite_Right_5);
            ship.Name = shipInfo.Name;
            ship.Experience = (long)shipInfo.Experience;
            ship.DataChanged = false;
            return ship;
        }

        #endregion

        public static ItemQuality Quality(this IShipModel shipModel)
        {
            var modsCount = shipModel.Modifications.Count;
            if (modsCount >= 5)
                return ItemQuality.Legend;
            if (modsCount >= 4)
                return ItemQuality.Epic;
            if (modsCount >= 3)
                return ItemQuality.Perfect;
            if (modsCount >= 2)
                return ItemQuality.High;
            if (modsCount >= 1)
                return ItemQuality.Medium;

            return ItemQuality.Common;
        }

        public static IShip Unlocked(this IShip ship)
        {
            foreach (var item in ship.Components)
                item.Locked = false;

            return ship;
        }

        public static IShip OfLevel(this IShip ship, int level)
        {
            ship.Experience = Experience.FromLevel(level);
            return ship;
        }

        public static IEnumerable<IShip> Create(this IEnumerable<ShipBuild> ships, int requiredLevel, Random random, IDatabase database)
        {
            return ships.Select(item => global::Model.Factories.Ship.Create(item, requiredLevel, random, database));
        }
    }
}
