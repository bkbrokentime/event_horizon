using Combat.Collision;
using Combat.Component.Unit;
using Combat.Factory;
using DebugLogSetting;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Stats
{
    public class DamageIndicator : IDamageIndicator
    {
        public DamageIndicator(IUnit unit, EffectFactory effectFactory, float opacity = 1.0f)
        {
            _effectFactory = effectFactory;
            _unit = unit;
            _kineticDamageColor = new Color(0.5f, 1.0f, 0.5f, opacity);
            _heatDamageColor = new Color(1, 1, 0, opacity);
            _energyDamageColor = new Color(0, 1, 1, opacity);
            _flameDamageColor = new Color(1f, 0.5f, 0, opacity);
            _antimatterDamageColor = new Color(0.25f, 0, 0.25f, opacity);
            _corrosionDamageColor = new Color(0, 0.75f, 0, opacity);
            _quantumDamageColor = new Color(0.75f, 0, 0.375f, opacity);
            _directDamageColor = new Color(1, 0.5f, 1, opacity);
            _darkmatterDamageColor = new Color(0.3f, 0, 0, opacity);
            _darkenergyDamageColor = new Color(0.15f, 0, 0.3f, opacity);
            _annihilationDamageColor = new Color(0.375f, 0, 0.75f, opacity);
            _shieldDamageColor = new Color(0.5f, 0.5f, 0.5f, opacity);
        }

        public void ApplyDamage(Impact damage)
        {
            if (damage.AllDamageData.KineticDamage.Value > 0)
            {
                _kineticChanged = true;
                _kinetic += damage.AllDamageData.KineticDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._kinetic has changed by: " + damage.AllDamageData.KineticDamage.Value + "  Now:  " + _kinetic);
            }

            if (damage.AllDamageData.HeatDamage.Value > 0)
            {
                _heatChanged = true;
                _heat += damage.AllDamageData.HeatDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._heat has changed by: " + damage.AllDamageData.HeatDamage.Value + "  Now:  " + _heat);
            }

            if (damage.AllDamageData.EnergyDamage.Value > 0)
            {
                _energyChanged = true;
                _energy += damage.AllDamageData.EnergyDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._energy has changed by: " + damage.AllDamageData.EnergyDamage.Value + "  Now:  " + _energy);
            }

            if (damage.AllDamageData.DirectDamage.Value > 0)
            {
                _directChanged = true;
                _direct += damage.AllDamageData.DirectDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._direct has changed by: " + damage.AllDamageData.DirectDamage.Value + "  Now:  " + _direct);
            }

            if (damage.AllDamageData.FlameDamage.Value > 0)
            {
                _flameChanged = true;
                _flame += damage.AllDamageData.FlameDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._flame has changed by: " + damage.AllDamageData.FlameDamage.Value + "  Now:  " + _flame);
            }

            if (damage.AllDamageData.AntimatterDamage.Value > 0)
            {
                _antimatterChanged = true;
                _antimatter += damage.AllDamageData.AntimatterDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._antimatter has changed by: " + damage.AllDamageData.AntimatterDamage.Value + "  Now:  " + _antimatter);
            }

            if (damage.AllDamageData.CorrosionDamage.Value > 0)
            {
                _corrosionChanged = true;
                _corrosion += damage.AllDamageData.CorrosionDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._corrosion has changed by: " + damage.AllDamageData.CorrosionDamage.Value + "  Now:  " + _corrosion);
            }

            if (damage.AllDamageData.QuantumDamage.Value > 0)
            {
                _quantumChanged = true;
                _quantum += damage.AllDamageData.QuantumDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._quantum has changed by: " + damage.AllDamageData.QuantumDamage.Value + "  Now:  " + _quantum);
            }

            if (damage.AllDamageData.DarkmatterDamage.Value > 0)
            {
                _darkmatterChanged = true;
                _darkmatter += damage.AllDamageData.DarkmatterDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._darkmatter has changed by: " + damage.AllDamageData.DarkmatterDamage.Value + "  Now:  " + _darkmatter);
            }

            if (damage.AllDamageData.DarkenergyDamage.Value > 0)
            {
                _darkenergyChanged = true;
                _darkenergy += damage.AllDamageData.DarkenergyDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._darkenergy has changed by: " + damage.AllDamageData.DarkenergyDamage.Value + "  Now:  " + _darkenergy);
            }

            if (damage.AllDamageData.AnnihilationDamage.Value > 0)
            {
                _annihilationChanged = true;
                _annihilation += damage.AllDamageData.AnnihilationDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._annihilation has changed by: " + damage.AllDamageData.AnnihilationDamage.Value + "  Now:  " + _annihilation);
            }

            if (damage.AllDamageData.ShieldDamage.Value > 0)
            {
                _shieldChanged = true;
                _shield += damage.AllDamageData.ShieldDamage.Value;
                if (DamageDebugLogSetting.DamageDebugLog)
                    Debug.Log("DamageIndicator._shield has changed by: " + damage.AllDamageData.ShieldDamage.Value + "  Now:  " + _shield);
            }
        }

        public void Dispose()
        {
            if (_kinetic > 1f)
                CreateDamageEffect(_kinetic, _kineticDamageColor);
            if (_heat > 1f)
                CreateDamageEffect(_heat, _heatDamageColor);
            if (_energy > 1f)
                CreateDamageEffect(_energy, _energyDamageColor);
            if (_direct > 1f)
                CreateDamageEffect(_direct, _directDamageColor);

            if (_flame > 1f)
                CreateDamageEffect(_flame, _flameDamageColor);
            if (_antimatter > 1f)
                CreateDamageEffect(_antimatter, _antimatterDamageColor);
            if (_corrosion > 1f)
                CreateDamageEffect(_corrosion, _corrosionDamageColor);
            if (_quantum > 1f)
                CreateDamageEffect(_quantum, _quantumDamageColor);
            if (_darkmatter > 1f)
                CreateDamageEffect(_darkmatter, _darkmatterDamageColor);
            if (_darkenergy > 1f)
                CreateDamageEffect(_darkenergy, _darkenergyDamageColor);
            if (_annihilation > 1f)
                CreateDamageEffect(_annihilation, _annihilationDamageColor);

            if (_shield > 1f)
                CreateDamageEffect(_shield, _shieldDamageColor);
        }

        public void Update(float elapsedTime)
        {
            _currentTime += elapsedTime;
            if (_currentTime - _lastShowTime > _cooldown)
            {
                _lastShowTime = _currentTime;

                if (_kinetic > 1f && !_kineticChanged)
                {
                    CreateDamageEffect(_kinetic, _kineticDamageColor);
                    _kinetic = 0;
                }
                if (_heat > 1f && !_heatChanged)
                {
                    CreateDamageEffect(_heat, _heatDamageColor);
                    _heat = 0;
                }
                if (_energy > 1f && !_energyChanged)
                {
                    CreateDamageEffect(_energy, _energyDamageColor);
                    _energy = 0;
                }
                if (_direct > 1f && !_directChanged)
                {
                    CreateDamageEffect(_direct, _directDamageColor);
                    _direct = 0;
                }

                if (_flame > 1f && !_flameChanged)
                {
                    CreateDamageEffect(_flame, _flameDamageColor);
                    _flame = 0;
                }
                if (_antimatter > 1f && !_antimatterChanged)
                {
                    CreateDamageEffect(_antimatter, _antimatterDamageColor);
                    _antimatter = 0;
                }
                if (_corrosion > 1f && !_corrosionChanged)
                {
                    CreateDamageEffect(_corrosion, _corrosionDamageColor);
                    _corrosion = 0;
                }
                if (_quantum > 1f && !_quantumChanged)
                {
                    CreateDamageEffect(_quantum, _quantumDamageColor);
                    _quantum = 0;
                }
                if (_darkmatter > 1f && !_darkmatterChanged)
                {
                    CreateDamageEffect(_darkmatter, _darkmatterDamageColor);
                    _darkmatter = 0;
                }
                if (_darkenergy > 1f && !_darkenergyChanged)
                {
                    CreateDamageEffect(_darkenergy, _darkenergyDamageColor);
                    _darkenergy = 0;
                }
                if (_annihilation > 1f && !_annihilationChanged)
                {
                    CreateDamageEffect(_annihilation, _annihilationDamageColor);
                    _annihilation = 0;
                }

                if (_shield > 1f && !_shieldChanged)
                {
                    CreateDamageEffect(_shield, _shieldDamageColor);
                    _shield = 0;
                }
            }

            _kineticChanged = false;
            _heatChanged = false;
            _energyChanged = false;
            _directChanged = false;
            _flameChanged = false;
            _antimatterChanged = false;
            _corrosionChanged = false;
            _quantumChanged = false;
            _darkmatterChanged = false;
            _darkenergyChanged = false;
            _annihilationChanged = false;
            _shieldChanged = false;
        }
        
        private void CreateDamageEffect(float damage, Color color)
        {
            _effectFactory.CreateDamageTextEffect(damage, color, _unit.Body.Position, _unit.Body.Velocity);
        }

        private float _kinetic;
        private float _heat;
        private float _energy;
        private float _direct;

        private float _flame;
        private float _antimatter;
        private float _corrosion;
        private float _quantum;
        private float _darkmatter;
        private float _darkenergy;
        private float _annihilation;

        private float _shield;

        private bool _kineticChanged;
        private bool _heatChanged;
        private bool _energyChanged;
        private bool _directChanged;

        private bool _flameChanged;
        private bool _antimatterChanged;
        private bool _corrosionChanged;
        private bool _quantumChanged;
        private bool _darkmatterChanged;
        private bool _darkenergyChanged;
        private bool _annihilationChanged;

        private bool _shieldChanged;

        private float _lastShowTime;
        private float _currentTime;

        private const float _cooldown = 0.02f;

        private readonly Color _kineticDamageColor;
        private readonly Color _heatDamageColor;
        private readonly Color _energyDamageColor;
        private readonly Color _directDamageColor;
        private readonly Color _shieldDamageColor;
        private readonly Color _flameDamageColor;
        private readonly Color _antimatterDamageColor;
        private readonly Color _corrosionDamageColor;
        private readonly Color _quantumDamageColor;
        private readonly Color _darkmatterDamageColor;
        private readonly Color _darkenergyDamageColor;
        private readonly Color _annihilationDamageColor;

        private readonly EffectFactory _effectFactory;
        private readonly IUnit _unit;
    }
}
