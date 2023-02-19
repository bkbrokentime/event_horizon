using UnityEngine;
using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;

namespace Constructor
{
	public interface IShipSpecification
	{
	    ShipType Type { get; }
	    Model.IShipStats Stats { get; }
		IEnumerable<IWeaponPlatformData> Platforms { get; }
		IEnumerable<IUAVPlatformData> UAVPlatforms { get; }
		IEnumerable<IDeviceData> Devices { get; }
		IEnumerable<IDroneBayData> DroneBays { get; }
        bool isdrone { get; set; }
	}

    public struct ShipType
    {
        public ShipType(ItemId<Ship> id, DifficultyClass shipClass,EnhancementLevel shipEnhance, int level, int size)
        {
            Id = id;
            Class = shipClass;
            Enhance = shipEnhance;
            Size = size;
            Level = level;
        }

        public readonly ItemId<Ship> Id;
        public readonly DifficultyClass Class;
        public readonly EnhancementLevel Enhance;
        public readonly int Size;
        public readonly int Level;
    }

    public interface IWeaponPlatformData
    {
        Vector2 Position { get; set; }
        float Rotation { get; }
        float Offset { get; }
        float Size { get; }
        SpriteId Image { get; }
        float AutoAimingArc { get; }
        float RotationSpeed { get; }
        Vector2 MoveCenterPosition { get; }
        float MoveSpeed { get; }
        Vector2 MoveCenterRange { get; }
        ICompanionData Companion { get; }
        IEnumerable<IWeaponData> Weapons { get; }
        IEnumerable<IWeaponDataObsolete> WeaponsObsolete { get; }
    }
	public interface IUAVPlatformData
	{
		Vector2 Position { get; }
		float Rotation { get; }
		float BaseSpread { get; }
        ICompanionData Companion { get; }

    }
    public interface ICompanionData
    {
        Satellite Satellite { get; }
        CompanionLocation Location { get; }
        float Weight { get; }
        SatelliteFormation Formation { get; }
    }
    public interface IWeaponData
    {
        Weapon Weapon { get; }
        Ammunition Ammunition { get; }
        WeaponStatModifier Stats { get; }
        int KeyBinding { get; }
    }

    public struct WeaponStatModifier
    {
        public StatMultiplier DamageMultiplier;
        public StatMultiplier RangeMultiplier;
        public StatMultiplier LifetimeMultiplier;
        public StatMultiplier FireRateMultiplier;
        public StatMultiplier EnergyCostMultiplier;
        public StatMultiplier AoeRadiusMultiplier;
        public StatMultiplier VelocityMultiplier;
        public StatMultiplier WeightMultiplier;
        public StatMultiplier SpreadMultiplier;
        public StatMultiplier MagazineMultiplier;
        public Color? Color;
    }

    public interface IWeaponDataObsolete
	{
		WeaponStats Weapon { get; }
        AmmunitionObsoleteStats Ammunition { get; }
        int KeyBinding { get; }
	}
	
	public interface IDeviceData
	{
		DeviceStats Device { get; }
		int KeyBinding { get; }
	}
	
    public enum DroneBehaviour { Aggressive = 0, Defensive = 1 }

	public interface IDroneBayData
	{
		DroneBayStats DroneBay { get; }
		ShipBuild Drone { get; }
		int KeyBinding { get; }
        DroneBehaviour Behaviour { get; }
	}
}
