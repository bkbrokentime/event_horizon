using System;
using Combat.Component.Body;
using DebugLogSetting;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Collision
{
    public class Impulse
    {
        public Impulse()
        {
            _values = new Vector2[8];
            _count = 0;
        }

        public void Apply(IBody body)
        {
            for (var i = 0; i < _count; ++i)
                body.ApplyForce(_values[i * 2], _values[i * 2 + 1]);
        }

        public void Append(Vector2 position, Vector2 impulse)
        {
            if (_count + 2 >= _values.Length)
                Array.Resize(ref _values, _count + 2);

            _values[_count++] = position;
            _values[_count++] = impulse;
        }

        public Impulse Append(Impulse other)
        {
            if (other == null || other._count == 0)
                return this;

            if (_count + other._count >= _values.Length)
                Array.Resize(ref _values, _count + other._count);

            Array.Copy(other._values, 0, _values, _count, other._count);
            _count += other._count;

            return this;
        }

        public void Clear()
        {
            _count = 0;
        }

        private int _count;
        private Vector2[] _values;
    }
    public struct Impact
    {
        public AllDamageData AllDamageData;
        public int Flame;

        public int Corrosion;


        public Impulse Impulse;
        public CollisionEffect Effects;
        public Impact(AllDamageData allDamageData, int flame, int corrosion, Impulse impulse, CollisionEffect collisionEffect)
        {
            AllDamageData = allDamageData;
            Flame = flame;
            Corrosion = corrosion;
            Impulse = impulse;
            Effects = collisionEffect;
        }
        public void AddDamage(DamageType type, float amount)
        {
            if (amount < 0)
                throw new InvalidOperationException();

            if (type == DamageType.Direct)
                AllDamageData.DirectDamage.UpDateDamage(amount);
            else if (type == DamageType.Impact)
                AllDamageData.KineticDamage.UpDateDamage(amount);
            else if (type == DamageType.Energy)
                AllDamageData.EnergyDamage.UpDateDamage(amount);
            else if (type == DamageType.Heat)
                AllDamageData.HeatDamage.UpDateDamage(amount);
            else if (type == DamageType.Flame)
            {
                AllDamageData.FlameDamage.UpDateDamage(amount);
                Flame++;
            }
            else if (type == DamageType.Antimatter)
                AllDamageData.AntimatterDamage.UpDateDamage(amount);
            else if (type == DamageType.Corrosion)
            {
                AllDamageData.CorrosionDamage.UpDateDamage(amount);
                Corrosion++;
            }
            else if (type == DamageType.Quantum)
                AllDamageData.QuantumDamage.UpDateDamage(amount);
            else if (type == DamageType.Darkmatter)
                AllDamageData.DarkmatterDamage.UpDateDamage(amount);
            else if (type == DamageType.Darkenergy)
                AllDamageData.DarkenergyDamage.UpDateDamage(amount);
            else if (type == DamageType.Annihilation)
                AllDamageData.AnnihilationDamage.UpDateDamage(amount * 3);
            else
                throw new System.ArgumentException("unknown damage type");

            if(DamageDebugLogSetting.DamageDebugLog)
            Debug.Log("AddDamage " + type.ToString() + "  amount:  " + amount);
            //AllDamageData.DebugList();
        }
        public float GetTotalDamage(Resistance resistance)
        {
            var damage =
                AllDamageData.KineticDamage.GetArmorDamage() * (1f - resistance.Kinetic) +
                AllDamageData.EnergyDamage.GetArmorDamage() * (1f - resistance.Energy) +
                AllDamageData.HeatDamage.GetArmorDamage() * (1f - resistance.Heat) +
                AllDamageData.DirectDamage.GetArmorDamage() +

                AllDamageData.FlameDamage.GetArmorDamage() * (1f - resistance.Heat) +
                AllDamageData.AntimatterDamage.GetArmorDamage() +
                AllDamageData.CorrosionDamage.GetArmorDamage() +
                AllDamageData.QuantumDamage.GetArmorDamage() * (1f - resistance.Quantum) +

                AllDamageData.DarkmatterDamage.GetArmorDamage() +
                AllDamageData.DarkenergyDamage.GetArmorDamage() +
                AllDamageData.AnnihilationDamage.GetArmorDamage()
                ;
            return damage;
        }
        public float ArmorGetTotalDamage(Resistance resistance)
        {
            var damage =
                AllDamageData.KineticDamage.GetArmorDamage() * (1f - resistance.Kinetic) +
                AllDamageData.EnergyDamage.GetArmorDamage() * (1f - resistance.Energy) +
                AllDamageData.HeatDamage.GetArmorDamage() * (1f - resistance.Heat) +
                AllDamageData.DirectDamage.GetArmorDamage() +

                AllDamageData.FlameDamage.GetArmorDamage() * (1f + Mathf.Clamp(Flame - 1, 0, 300) * 0.01f) * (1f - resistance.Heat) +
                AllDamageData.AntimatterDamage.GetArmorDamage() +
                AllDamageData.CorrosionDamage.GetArmorDamage() * (1f + Mathf.Clamp(Corrosion - 1, 0, 200) * 0.025f) +
                AllDamageData.QuantumDamage.GetArmorDamage() * (1f - resistance.Quantum) +

                AllDamageData.DarkmatterDamage.GetArmorDamage() +
                AllDamageData.DarkenergyDamage.GetArmorDamage() +
                AllDamageData.AnnihilationDamage.GetArmorDamage()
                ;
            return damage;
        }
        public float ShieldGetTotalDamage(Resistance resistance)
        {
            var damage =
                AllDamageData.KineticDamage.GetShieldDamage() * (1f - resistance.ShieldKinetic) +
                AllDamageData.EnergyDamage.GetShieldDamage() * (1f - resistance.ShieldEnergy) +
                AllDamageData.HeatDamage.GetShieldDamage() * (1f - resistance.ShieldHeat) +
                AllDamageData.DirectDamage.GetShieldDamage() +

                //AllDamageData.FlameDamage.GetShieldDamage() * (1f + Mathf.Clamp(Flame - 1, 0, 300) * 0.01f) * (1f - resistance.ShieldHeat) +
                AllDamageData.AntimatterDamage.GetShieldDamage() +
                //AllDamageData.CorrosionDamage.GetShieldDamage() * (1f + Mathf.Clamp(Corrosion - 1, 0, 200) * 0.025f) +
                AllDamageData.QuantumDamage.GetShieldDamage() * (1f - resistance.ShieldQuantum) +

                AllDamageData.DarkmatterDamage.GetShieldDamage() +
                AllDamageData.DarkenergyDamage.GetShieldDamage() +
                AllDamageData.AnnihilationDamage.GetShieldDamage()
                ;
            return damage;
        }
        public float EnergyShieldGetTotalDamage(Resistance resistance)
        {
            var damage =
                AllDamageData.KineticDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldKinetic) +
                AllDamageData.EnergyDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldEnergy) +
                AllDamageData.HeatDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldHeat) +
                AllDamageData.DirectDamage.GetEnergyShieldDamage() +

                AllDamageData.FlameDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldHeat) +
                AllDamageData.AntimatterDamage.GetEnergyShieldDamage() +
                AllDamageData.CorrosionDamage.GetEnergyShieldDamage() +
                AllDamageData.QuantumDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldQuantum) +

                AllDamageData.DarkmatterDamage.GetEnergyShieldDamage() +
                AllDamageData.DarkenergyDamage.GetEnergyShieldDamage() +
                AllDamageData.AnnihilationDamage.GetEnergyShieldDamage()
                ;
            return damage;
        }

        public void EnergyShieldReduceQuantumDamage(Resistance resistance)
        {
            AllDamageData.QuantumDamage.SetDamage(AllDamageData.QuantumDamage * (1f - resistance.EnergyShieldQuantum));
        }


        public void AddImpulse(Vector2 position, Vector2 impulse)
        {
            if (Impulse == null)
                Impulse = new Impulse();

            Impulse.Append(position, impulse);
        }

        public void ApplyImpulse(IBody body)
        {
            if (Impulse != null)
                Impulse.Apply(body);
        }

        public void RemoveImpulse()
        {
            if (Impulse != null)
                Impulse.Clear();
        }

        public Impact GetDamage(Resistance resistance)
        {
            var newimpact = new Impact(this.AllDamageData, this.Flame, this.Corrosion, this.Impulse, this.Effects);

            newimpact.AllDamageData.KineticDamage.SetDamage(this.AllDamageData.KineticDamage.GetArmorDamage() * (1f - resistance.Kinetic));
            newimpact.AllDamageData.EnergyDamage.SetDamage(this.AllDamageData.EnergyDamage.GetArmorDamage() * (1f - resistance.Energy));
            newimpact.AllDamageData.HeatDamage.SetDamage(this.AllDamageData.HeatDamage.GetArmorDamage() * (1f - resistance.Heat));
            newimpact.AllDamageData.DirectDamage.SetDamage(this.AllDamageData.DirectDamage.GetArmorDamage());

            newimpact.AllDamageData.FlameDamage.SetDamage(this.AllDamageData.FlameDamage.GetArmorDamage() * (1f - resistance.Heat));
            newimpact.AllDamageData.AntimatterDamage.SetDamage(this.AllDamageData.AntimatterDamage.GetArmorDamage());
            newimpact.AllDamageData.CorrosionDamage.SetDamage(this.AllDamageData.CorrosionDamage.GetArmorDamage());
            newimpact.AllDamageData.QuantumDamage.SetDamage(this.AllDamageData.QuantumDamage.GetArmorDamage() * (1f - resistance.Quantum));

            newimpact.AllDamageData.DarkmatterDamage.SetDamage(this.AllDamageData.DarkmatterDamage.GetArmorDamage());
            newimpact.AllDamageData.DarkenergyDamage.SetDamage(this.AllDamageData.DarkenergyDamage.GetArmorDamage());
            newimpact.AllDamageData.AnnihilationDamage.SetDamage(this.AllDamageData.AnnihilationDamage.GetArmorDamage());
            return newimpact;
        }

        public void ApplyShield(float power, Resistance resistance)
        {
            var damage = ShieldGetTotalDamage(resistance);
            if (damage <= 0 || power <= 0)
                return;

            if (damage <= power)
            {
                ShieldRemoveDamage();
                AllDamageData.ShieldDamage.UpDateDamage(damage);
            }
            else
            {
                AllDamageData.KineticDamage.UpDateDamage(-power * AllDamageData.KineticDamage.GetShieldDamage() / damage);
                AllDamageData.EnergyDamage.UpDateDamage(-power * AllDamageData.EnergyDamage.GetShieldDamage() / damage);
                AllDamageData.HeatDamage.UpDateDamage(-power * AllDamageData.HeatDamage.GetShieldDamage() / damage);
                AllDamageData.DirectDamage.UpDateDamage(-power * AllDamageData.DirectDamage.GetShieldDamage() / damage);

                //AllDamageData.FlameDamage .UpDateDamage(-power * AllDamageData.FlameDamage.GetShieldDamage() / damage);
                AllDamageData.AntimatterDamage.UpDateDamage(-power * AllDamageData.AntimatterDamage.GetShieldDamage() / damage);
                //AllDamageData.CorrosionDamage.UpDateDamage(-power * AllDamageData.CorrosionDamage.GetShieldDamage() / damage);
                AllDamageData.QuantumDamage.UpDateDamage(-power * AllDamageData.QuantumDamage.GetShieldDamage() / damage);

                AllDamageData.DarkmatterDamage.UpDateDamage(-power * AllDamageData.DarkmatterDamage.GetShieldDamage() / damage);
                AllDamageData.DarkenergyDamage.UpDateDamage(-power * AllDamageData.DarkenergyDamage.GetShieldDamage() / damage);
                AllDamageData.AnnihilationDamage.UpDateDamage(-power * AllDamageData.AnnihilationDamage.GetShieldDamage() / damage);

                AllDamageData.ShieldDamage.UpDateDamage(power);
            }
        }



        public void EnergyShieldRemoveDamage(float amount, Resistance resistance)
        {
            var total = EnergyShieldGetTotalDamage(resistance);
            if (total <= amount || total <= 0.000001f)
            {
                EnergyShieldRemoveDamage();
                return;
            }

            AllDamageData.KineticDamage.UpDateDamage(-amount * AllDamageData.KineticDamage.GetEnergyShieldDamage() / total);
            AllDamageData.EnergyDamage.UpDateDamage(-amount * AllDamageData.EnergyDamage.GetEnergyShieldDamage() / total);
            AllDamageData.HeatDamage.UpDateDamage(-amount * AllDamageData.HeatDamage.GetEnergyShieldDamage() / total);
            AllDamageData.DirectDamage.UpDateDamage(-amount * AllDamageData.DirectDamage.GetEnergyShieldDamage() / total);

            AllDamageData.FlameDamage.UpDateDamage(-amount * AllDamageData.FlameDamage.GetEnergyShieldDamage() / total);
            AllDamageData.AntimatterDamage.UpDateDamage(-amount * AllDamageData.AntimatterDamage.GetEnergyShieldDamage() / total);
            AllDamageData.CorrosionDamage.UpDateDamage(-amount * AllDamageData.CorrosionDamage.GetEnergyShieldDamage() / total);
            AllDamageData.QuantumDamage.UpDateDamage(-amount * AllDamageData.QuantumDamage.GetEnergyShieldDamage() / total);

            AllDamageData.DarkmatterDamage.UpDateDamage(-amount * AllDamageData.DarkmatterDamage.GetEnergyShieldDamage() / total);
            AllDamageData.DarkenergyDamage.UpDateDamage(-amount * AllDamageData.DarkenergyDamage.GetEnergyShieldDamage() / total);
            AllDamageData.AnnihilationDamage.UpDateDamage(-amount * AllDamageData.AnnihilationDamage.GetEnergyShieldDamage() / total);
        }
        public void RemoveArmorDamage()
        {
            RemoveTypeDamage(DefenseType.Armor);
        }
        public void ShieldRemoveDamage()
        {
            RemoveTypeDamage(DefenseType.Shield);
        }
        public void EnergyShieldRemoveDamage()
        {
            RemoveTypeDamage(DefenseType.EnergyShield);
        }
        public void RemoveTypeDamage(DefenseType type)
        {
            AllDamageData.KineticDamage.RemoveTypeDamage(type);
            AllDamageData.EnergyDamage.RemoveTypeDamage(type);
            AllDamageData.HeatDamage.RemoveTypeDamage(type);
            AllDamageData.DirectDamage.RemoveTypeDamage(type);

            AllDamageData.FlameDamage.RemoveTypeDamage(type);
            AllDamageData.AntimatterDamage.RemoveTypeDamage(type);
            AllDamageData.CorrosionDamage.RemoveTypeDamage(type);
            AllDamageData.QuantumDamage.RemoveTypeDamage(type);

            AllDamageData.DarkmatterDamage.RemoveTypeDamage(type);
            AllDamageData.DarkenergyDamage.RemoveTypeDamage(type);
            AllDamageData.AnnihilationDamage.RemoveTypeDamage(type);
        }



        public void DamageStorageRead_Flame(DamageStorage damageStorage)
        {
            Flame += damageStorage.Flame;
        }
        public void DamageStorageRead_Corrosion(DamageStorage damageStorage)
        {
            Corrosion += damageStorage.Corrosion;
        }


        public void DamageStorageRead(DamageStorage damageStorage)
        {
            //KineticDamage += damageStorage.KineticDamage;
            //EnergyDamage += damageStorage.EnergyDamage;
            //HeatDamage += damageStorage.HeatDamage;
            //DirectDamage += damageStorage.DirectDamage;

            //FlameDamage += damageStorage.FlameDamage;
            Flame += damageStorage.Flame;

            //AntimatterDamage += damageStorage.AntimatterDamage;

            //CorrosionDamage += damageStorage.CorrosionDamage;
            Corrosion += damageStorage.Corrosion;

            //QuantumDamage += damageStorage.QuantumDamage;

            //Repair += damageStorage.Repair;

            //ShieldDamage += damageStorage.ShieldDamage;
            //EnergyShieldDamage += damageStorage.EnergyShieldDamage;

            //EnergyDrain += damageStorage.EnergyDrain;

        }


        /*
        public void ApplyEnergyShield(float power, Resistance resistance)
        {
            var damage =
                AllDamageData.KineticDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldKinetic) +
                AllDamageData.EnergyDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldEnergy) +
                AllDamageData.HeatDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldHeat) +
                AllDamageData.DirectDamage.GetEnergyShieldDamage() +

                AllDamageData.FlameDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldHeat) +
                AllDamageData.AntimatterDamage.GetEnergyShieldDamage() +
                AllDamageData.CorrosionDamage.GetEnergyShieldDamage() +
                AllDamageData.QuantumDamage.GetEnergyShieldDamage() * (1f - resistance.EnergyShieldQuantum) +

                AllDamageData.DarkmatterDamage.GetEnergyShieldDamage() +
                AllDamageData.DarkenergyDamage.GetEnergyShieldDamage() +
                AllDamageData.AnnihilationDamage.GetEnergyShieldDamage()
                ;

            if (damage <= 0 || power <= 0)
                return;

            if (damage <= power)
            {
                EnergyShieldRemoveDamage();
                AllDamageData.EnergyShieldDamage.UpDateDamage(power);
            }
            else
            {
                AllDamageData.KineticDamage.UpDateDamage(-power * AllDamageData.KineticDamage.GetShieldDamage() / damage);
                AllDamageData.EnergyDamage.UpDateDamage(-power * AllDamageData.EnergyDamage.GetShieldDamage() / damage);
                AllDamageData.HeatDamage.UpDateDamage(-power * AllDamageData.HeatDamage.GetShieldDamage() / damage);
                AllDamageData.DirectDamage.UpDateDamage(-power * AllDamageData.DirectDamage.GetShieldDamage() / damage);

                AllDamageData.FlameDamage.UpDateDamage(-power * AllDamageData.FlameDamage.GetShieldDamage() / damage);
                AllDamageData.AntimatterDamage.UpDateDamage(-power * AllDamageData.AntimatterDamage.GetShieldDamage() / damage);
                AllDamageData.CorrosionDamage.UpDateDamage(-power * AllDamageData.CorrosionDamage.GetShieldDamage() / damage);
                AllDamageData.QuantumDamage.UpDateDamage(-power * AllDamageData.QuantumDamage.GetShieldDamage() / damage);

                AllDamageData.DarkmatterDamage.UpDateDamage(-power * AllDamageData.DarkmatterDamage.GetShieldDamage() / damage);
                AllDamageData.DarkenergyDamage.UpDateDamage(-power * AllDamageData.DarkenergyDamage.GetShieldDamage() / damage);
                AllDamageData.AnnihilationDamage.UpDateDamage(-power * AllDamageData.AnnihilationDamage.GetShieldDamage() / damage);

                AllDamageData.EnergyShieldDamage.UpDateDamage(power);
            }
        }
        */
        /*
        public void Append(Impact second)
        {
            AllDamageData.KineticDamage.UpDateDamage(second.AllDamageData.KineticDamage.Value);
            AllDamageData.EnergyDamage.UpDateDamage(second.AllDamageData.KineticDamage.Value);
            AllDamageData.HeatDamage.UpDateDamage(second.AllDamageData.KineticDamage.Value);
            AllDamageData.DirectDamage.UpDateDamage(second.AllDamageData.KineticDamage.Value);

            AllDamageData.AntimatterDamage.UpDateDamage(second.AllDamageData.KineticDamage.Value);
            AllDamageData.QuantumDamage.UpDateDamage(second.AllDamageData.KineticDamage.Value);

            AllDamageData.ShieldDamage.UpDateDamage(second.AllDamageData.KineticDamage.Value);
            AllDamageData.Repair.UpDateDamage(second.AllDamageData.KineticDamage.Value);
            Effects |= second.Effects;
            Impulse = Impulse == null ? second.Impulse : Impulse.Append(second.Impulse);
        }
        */
        /*
        public void RemoveDamage(float amount, Resistance resistance)
        {
            var total = GetTotalDamage(resistance);
            if (total <= amount || total <= 0.000001f)
            {
                RemoveDamage();
                return;
            }

            AllDamageData.KineticDamage.UpDateDamage(-amount * AllDamageData.KineticDamage.GetArmorDamage() / total);
            AllDamageData.EnergyDamage.UpDateDamage(-amount * AllDamageData.EnergyDamage.GetArmorDamage() / total);
            AllDamageData.HeatDamage.UpDateDamage(-amount * AllDamageData.HeatDamage.GetArmorDamage() / total);
            AllDamageData.DirectDamage.UpDateDamage(-amount * AllDamageData.DirectDamage.GetArmorDamage() / total);

            AllDamageData.FlameDamage.UpDateDamage(-amount * AllDamageData.FlameDamage.GetArmorDamage() / total);
            AllDamageData.AntimatterDamage.UpDateDamage(-amount * AllDamageData.AntimatterDamage.GetArmorDamage() / total);
            AllDamageData.CorrosionDamage.UpDateDamage(-amount * AllDamageData.CorrosionDamage.GetArmorDamage() / total);
            AllDamageData.QuantumDamage.UpDateDamage(-amount * AllDamageData.QuantumDamage.GetArmorDamage() / total);

            AllDamageData.DarkmatterDamage.UpDateDamage(-amount * AllDamageData.DarkmatterDamage.GetArmorDamage() / total);
            AllDamageData.DarkenergyDamage.UpDateDamage(-amount * AllDamageData.DarkenergyDamage.GetArmorDamage() / total);
            AllDamageData.AnnihilationDamage.UpDateDamage(-amount * AllDamageData.AnnihilationDamage.GetArmorDamage() / total);
        }
        */
    }
}   

