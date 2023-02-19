using UnityEngine;
using GameDatabase.Enums;
using Combat.Collision;
using DebugLogSetting;

namespace GameDatabase.Model
{
    public enum AllDamageType
    {
        Impact = DamageType.Impact,
        Energy = DamageType.Energy,
        Heat = DamageType.Heat,
        Direct = DamageType.Direct,
        Flame = DamageType.Flame,
        Antimatter = DamageType.Antimatter,
        Corrosion = DamageType.Corrosion,
        Quantum = DamageType.Quantum,
        Darkmatter = DamageType.Darkmatter,
        Darkenergy = DamageType.Darkenergy,
        Annihilation = DamageType.Annihilation,
        Repair,
        ShieldDamage,
        EnergyShieldDamage,
        EnergyDrain,
    }
    public enum DefenseType
    {
        Armor,
        Shield,
        EnergyShield,
        Other,
    }

    public struct DamageData
    {
        public DamageData(AllDamageType type, float value, float hitArmorMultiplier, float hitShieldMultiplier, float hitEnergyShieldMultiplier, float hitShieldPenetration, float hitEnergyShieldPenetration)
        {
            _damagetype = type;
            _value = value;
            _hitArmorMultiplier = hitArmorMultiplier;
            _hitShieldMultiplier = hitShieldMultiplier;
            _hitEnergyShieldMultiplier = hitEnergyShieldMultiplier;
            _hitShieldPenetration = hitShieldPenetration;
            _hitEnergyShieldPenetration = hitEnergyShieldPenetration;
        }

        public AllDamageType Type { get { return _damagetype; } }
        public float Value { get { return _value; } set { _value = value; } }
        public float HitArmorMultiplier { get { return _hitArmorMultiplier + 1f; } }
        public float HitShieldMultiplier { get { return _hitShieldMultiplier + 1f; } }
        public float HitEnergyShieldMultiplier { get { return _hitEnergyShieldMultiplier + 1f; } }
        public float HitShieldPenetration { get { return _hitShieldPenetration; } }
        public float HitEnergyShieldPenetration { get { return _hitEnergyShieldPenetration; } }

        public static float operator +(DamageData first, float second)
        {
            return first.Value + second;
        }
        public static float operator -(DamageData first, float second)
        {
            return first.Value - second;
        }
        public static float operator *(DamageData first, float second)
        {
            return first.Value * second;
        }
        public static float operator /(DamageData first, float second)
        {
            return first.Value / second;
        }
        public float GetArmorDamage() { return _value * HitArmorMultiplier; }
        public float GetShieldDamage() { return _value * HitShieldMultiplier * (1 - HitShieldPenetration); }
        public float GetEnergyShieldDamage() { return _value * HitEnergyShieldMultiplier * (1 - HitEnergyShieldPenetration); }
        public float GetArmorDamageWithoutMultiplier() { return _value; }
        public float GetShieldDamageWithoutMultiplier() { return _value * (1 - HitShieldPenetration); }
        public float GetEnergyShieldDamageWithoutMultiplier() { return _value * (1 - HitEnergyShieldPenetration); }
        public float GetMaxDamage() { return Mathf.Max(GetArmorDamage(), GetShieldDamage(), GetEnergyShieldDamage()); }
        public float GetMinDamage() { return Mathf.Min(GetArmorDamage(), GetShieldDamage(), GetEnergyShieldDamage()); }
        public float GetMaxDamageWithoutMultiplier() { return Mathf.Max(GetArmorDamageWithoutMultiplier(), GetShieldDamageWithoutMultiplier(), GetEnergyShieldDamageWithoutMultiplier()); }
        public float GetMinDamageWithoutMultiplier() { return Mathf.Min(GetArmorDamageWithoutMultiplier(), GetShieldDamageWithoutMultiplier(), GetEnergyShieldDamageWithoutMultiplier()); }
        public float GetTypeDamage(DefenseType type)
        {
            switch(type)
            {
                case DefenseType.Armor:
                    return GetArmorDamage();
                case DefenseType.Shield:
                    return GetShieldDamage();
                case DefenseType.EnergyShield:
                    return GetEnergyShieldDamage();
                case DefenseType.Other:
                    return GetMaxDamage();
                default:
                    return GetMinDamage();
            }
        }
        public float GetTypeDamageWithoutMultiplier(DefenseType type)
        {
            switch(type)
            {
                case DefenseType.Armor:
                    return GetArmorDamageWithoutMultiplier();
                case DefenseType.Shield:
                    return GetShieldDamageWithoutMultiplier();
                case DefenseType.EnergyShield:
                    return GetEnergyShieldDamageWithoutMultiplier();
                case DefenseType.Other:
                    return GetMaxDamageWithoutMultiplier();
                default:
                    return GetMinDamageWithoutMultiplier();
            }
        }
        public float GetTypeMultiplier(DefenseType type)
        {
            switch(type)
            {
                case DefenseType.Armor:
                    return HitArmorMultiplier;
                case DefenseType.Shield:
                    return HitShieldMultiplier;
                case DefenseType.EnergyShield:
                    return HitEnergyShieldMultiplier;
                case DefenseType.Other:
                    return 1;
                default:
                    return 1;
            }
        }
        public void UpDateDamage(float value) { _value += value; }
        public void SetDamage(float value) { _value = value; }
        public void RemoveTypeDamage(DefenseType type)
        {
            UpDateDamage(-GetTypeDamageWithoutMultiplier(type));
            if (_value < 0.000001f)
                RemoveAllDamage();
        }
        public void RemoveAllDamage() { _value = 0; }
        public void DebugList()
        {
            if (DamageDebugLogSetting.DamageDebugLog)
            {
                Debug.Log("Type:  " + Type.ToString() + "    Value:  " + Value);
            }
        }
        public void AllDebugList()
        {
            if (DamageDebugLogSetting.DamageDebugLog)
            {
                Debug.Log("Type:  " + Type.ToString() + "    Value:  " + Value);
                Debug.Log("Type:  " + Type.ToString() + "    HitArmorMultiplier:          " + HitArmorMultiplier);
                Debug.Log("Type:  " + Type.ToString() + "    HitShieldMultiplier:         " + HitShieldMultiplier);
                Debug.Log("Type:  " + Type.ToString() + "    HitEnergyShieldMultiplier:   " + HitEnergyShieldMultiplier);
                Debug.Log("Type:  " + Type.ToString() + "    HitShieldPenetration:        " + HitShieldPenetration);
                Debug.Log("Type:  " + Type.ToString() + "    HitEnergyShieldPenetration:  " + HitEnergyShieldPenetration);
            }
        }

        private AllDamageType _damagetype;
        private float _value;
        private float _hitArmorMultiplier;
        private float _hitShieldMultiplier;
        private float _hitEnergyShieldMultiplier;
        private float _hitShieldPenetration;
        private float _hitEnergyShieldPenetration;

    }

    public class AllDamageData
    {
        public DamageData KineticDamage;
        public DamageData EnergyDamage;
        public DamageData HeatDamage;
        public DamageData DirectDamage;

        public DamageData FlameDamage;
        public DamageData AntimatterDamage;
        public DamageData CorrosionDamage;
        public DamageData QuantumDamage;

        public DamageData DarkmatterDamage;
        public DamageData DarkenergyDamage;
        public DamageData AnnihilationDamage;

        public DamageData Repair;

        public DamageData ShieldDamage;
        public DamageData EnergyShieldDamage;

        public DamageData EnergyDrain;
        public AllDamageData()
        {
            KineticDamage = new DamageData(AllDamageType.Impact, 0, 0.5f, -0.5f, 0, 0, 0);
            EnergyDamage = new DamageData(AllDamageType.Energy, 0, -0.5f, 0.5f, 0, 0, 0);
            HeatDamage = new DamageData(AllDamageType.Heat, 0, 0, 0, 0, 0, 0);
            DirectDamage = new DamageData(AllDamageType.Direct, 0, 0, 0, 0, 0, 0);

            FlameDamage = new DamageData(AllDamageType.Flame, 0, 0, 0, 0, 1f, 0);
            AntimatterDamage = new DamageData(AllDamageType.Antimatter, 0, 2f, 1f, 0.5f, 0, 0);
            CorrosionDamage = new DamageData(AllDamageType.Corrosion, 0, 0, 0, 0, 1f, 0);
            QuantumDamage = new DamageData(AllDamageType.Quantum, 0, 0, 0, 0, 0, 1f);

            DarkmatterDamage = new DamageData(AllDamageType.Darkmatter, 0, 0, 0, 0, 0, 0.7f);
            DarkenergyDamage = new DamageData(AllDamageType.Darkenergy, 0, 0, 0, 0, 0.7f, 0);
            AnnihilationDamage = new DamageData(AllDamageType.Annihilation, 0, 0, 0, 0, 0.5f, 1f / 3f);

            Repair = new DamageData(AllDamageType.Repair, 0, 0, 0, 0, 0, 0);
            ShieldDamage = new DamageData(AllDamageType.ShieldDamage, 0, 0, 0, 0, 0, 0);
            EnergyShieldDamage = new DamageData(AllDamageType.EnergyShieldDamage, 0, 0, 0, 0, 0, 0);
            EnergyDrain = new DamageData(AllDamageType.EnergyDrain, 0, 0, 0, 0, 0, 0);
        }
        
        public void RemoveAllDamage()
        {
            KineticDamage.RemoveAllDamage();
            EnergyDamage.RemoveAllDamage();
            HeatDamage.RemoveAllDamage();
            DirectDamage.RemoveAllDamage();

            FlameDamage.RemoveAllDamage();
            AntimatterDamage.RemoveAllDamage();
            CorrosionDamage.RemoveAllDamage();
            QuantumDamage.RemoveAllDamage();

            DarkmatterDamage.RemoveAllDamage();
            DarkenergyDamage.RemoveAllDamage();
            AnnihilationDamage.RemoveAllDamage();

            Repair.RemoveAllDamage();
            ShieldDamage.RemoveAllDamage();
            EnergyShieldDamage.RemoveAllDamage();
            EnergyDrain.RemoveAllDamage();
        }

        public void DebugList()
        {
            if (DamageDebugLogSetting.DamageDebugLog)
            {
                KineticDamage.DebugList();
                EnergyDamage.DebugList();
                HeatDamage.DebugList();
                DirectDamage.DebugList();

                FlameDamage.DebugList();
                AntimatterDamage.DebugList();
                CorrosionDamage.DebugList();
                QuantumDamage.DebugList();

                DarkmatterDamage.DebugList();
                DarkenergyDamage.DebugList();
                AnnihilationDamage.DebugList();

                Repair.DebugList();
                ShieldDamage.DebugList();
                EnergyShieldDamage.DebugList();
                EnergyDrain.DebugList();
            }
        }
        public void AllDebugList()
        {
            if (DamageDebugLogSetting.DamageDebugLog)
            {
                KineticDamage.AllDebugList();
                EnergyDamage.AllDebugList();
                HeatDamage.AllDebugList();
                DirectDamage.AllDebugList();

                FlameDamage.AllDebugList();
                AntimatterDamage.AllDebugList();
                CorrosionDamage.AllDebugList();
                QuantumDamage.AllDebugList();

                DarkmatterDamage.AllDebugList();
                DarkenergyDamage.AllDebugList();
                AnnihilationDamage.AllDebugList();

                Repair.AllDebugList();
                ShieldDamage.AllDebugList();
                EnergyShieldDamage.AllDebugList();
                EnergyDrain.AllDebugList();
            }
        }

    }
}
