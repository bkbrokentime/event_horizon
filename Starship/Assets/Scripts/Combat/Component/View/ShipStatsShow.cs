using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constructor.Model;
using Combat.Component.Stats;
using System;
using Combat.Component.Ship;
using Combat.Collision;

public class ShipStatsShow : MonoBehaviour
{

    public IShipStats stats;
    public ShipStats shipStats;

    [System.Serializable]
    public struct _Stats
    {
        public float ArmorPoints;
        public float EnergyPoints;
        public float ShieldPoints;
        public float EnergyShieldPoints;
    }
    [System.Serializable]
    public struct __Stats
    {
        public float ArmorPoints;
        public float EnergyPoints;
        public float ShieldPoints;
        public float EnergyShieldPoints;

        public float DamageMultiplier;
        public float RangeMultiplier;
        public float FireRateMultiplier;
        public float EnergyCostMultiplier;
    }

    public _Stats _stats;
    public __Stats __stats;

    // Start is called before the first frame update
    void Start()
    {
        _getstats(stats);
    }

    // Update is called once per frame
    void Update()
    {
        __getstats(shipStats);
    }

    void _getstats(IShipStats stats)
    {
        _stats.ArmorPoints = stats.ArmorPoints;
        _stats.EnergyPoints = stats.EnergyPoints;
        _stats.ShieldPoints = stats.ShieldPoints;
        _stats.EnergyShieldPoints = stats.EnergyShieldPoints;
    }
    void __getstats(ShipStats stats)
    {
        __stats.ArmorPoints = stats.Armor.Value;
        __stats.EnergyPoints = stats.Energy.Value;
        __stats.ShieldPoints = stats.Shield.Value;
        __stats.EnergyShieldPoints = stats.EnergyShield.Value;
        __stats.DamageMultiplier = stats.WeaponUpgrade.DamageMultiplier;
        __stats.RangeMultiplier = stats.WeaponUpgrade.RangeMultiplier;
        __stats.FireRateMultiplier = stats.WeaponUpgrade.FireRateMultiplier;
        __stats.EnergyCostMultiplier = stats.WeaponUpgrade.EnergyCostMultiplier;
    }
}
