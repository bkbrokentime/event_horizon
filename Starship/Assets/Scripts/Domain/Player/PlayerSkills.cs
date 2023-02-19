using System;
using System.Collections.Generic;
using System.Linq;
using Session;
using GameServices.Database;
using Services.Messenger;
using GameModel.Skills;
using Zenject;
using UnityEngine;

namespace GameServices.Player
{
    public sealed class PlayerSkills : GameServiceBase
    {
        [Inject] private readonly PlayerFleet _playerFleet;

        [Inject]
        public PlayerSkills(
            ISessionData session, 
            IMessenger messenger, 
            Skills skills,
            SessionDataLoadedSignal dataLoadedSignal, 
            SessionCreatedSignal sessionCreatedSignal)
            : base(dataLoadedSignal, sessionCreatedSignal)
        {
            _skills = skills;
            _session = session;
            _messenger = messenger;

            Experience.MaxExperience = Experience.FromLevel(skills.TotalSkills);
        }

        public int AvailablePoints
        {
            get
            {
                GetSkillLevels();
                return _experience.Level - _pointsSpent;
            }
        }

        public int PointsSpent { get { return _pointsSpent; } }

        public bool CanAdd(int id)
        {
            var info = _skills[id];
            if (info.IsEmpty)
                return false;

            return CanAddSkill(info);
        }

        public bool HasSkill(int id)
        {
            if (_session.Upgrades.HasSkill(id))
                return true;

            var info = _skills[id];
            if (info.IsEmpty)
                return false;

            return _skills.IsFree(info.Type);
        }

        public bool TryAdd(int id)
        {
            var info = _skills[id];
            if (info.IsEmpty)
                return false;

            if (!CanAddSkill(info))
                return false;

            var level = GetSkillLevels()[info.Type];
            UnityEngine.Debug.Log(info.Type + ": " + level + " -> " + (level + info.Multilpler));
            GetSkillLevels()[info.Type] = level + info.Multilpler;

            _session.Upgrades.AddSkill(id);
            _pointsSpent++;

            _messenger.Broadcast(EventType.PlayerSkillsChanged);

            return true;
        }

        public void Reset()
        {
            _skillLevels = null;
            _pointsSpent = 0;
            _session.Upgrades.ResetSkills();
        }

        public void DebugGetAllValue()
        {
            var skill = GetSkillLevels();
            for(int i=0;i<skill.Count();i++)
            {
                Debug.Log(((SkillType)i).ToString() + ":  " + skill[(SkillType)i]);
            }
        }

        public Experience Experience
        {
            get { return _experience; }
            set
            {
                _experience = value;
                _session.Upgrades.PlayerExperience = value;
                _messenger.Broadcast(EventType.PlayerGainedExperience);
            }
        }

        public float ExperienceMultiplier { get { return 1.0f + GetSkillLevels()[SkillType.ShipExperience] * 0.1f; } }
        public float AttackMultiplier { get { return 1.0f + GetSkillLevels()[SkillType.ShipAttack] * 0.1f; } }
        public float DefenseMultiplier { get { return 1.0f + GetSkillLevels()[SkillType.ShipDefense] * 0.1f; } }
        public float ShieldStrengthBonus { get { return GetSkillLevels()[SkillType.ShieldStrength] * 0.1f; } }
        public float ShieldRechargeMultiplier { get { return 1.0f + GetSkillLevels()[SkillType.ShieldRecharge] * 0.1f; } }
        public float EnergyShieldStrengthBonus { get { return GetSkillLevels()[SkillType.EnergyShieldStrength] * 0.1f; } }
        public float EnergyShieldRechargeMultiplier { get { return 1.0f + GetSkillLevels()[SkillType.EnergyShieldRecharge] * 0.1f; } }
        public int MainFuelCapacity { get { return 100 + 50 * GetSkillLevels()[SkillType.MainFuelCapacity]; } }
        public float MainEnginePower { get { return 1f + 0.4f * GetSkillLevels()[SkillType.MainEnginePower]; } }
        public float MainFilghtRange { get { return 1.5f + 0.09f * GetSkillLevels()[SkillType.MainEnginePower]; } }
        public bool HasRescueUnit { get { return GetSkillLevels()[SkillType.MainRescueUnit] > 0; } }
        public float PlanetaryScanner { get { return 1.0f + GetSkillLevels()[SkillType.PlanetaryScanner] * 0.1f; } }
        public int SpaceScanner { get { return GetSkillLevels()[SkillType.SpaceScanner]; } }

        public float HeatResistance { get { return GetSkillLevels()[SkillType.HeatDefense] * 0.1f; } }
        public float EnergyResistance { get { return GetSkillLevels()[SkillType.EnergyDefense] * 0.1f; } }
        public float KineticResistance { get { return GetSkillLevels()[SkillType.KineticDefense] * 0.1f; } }
        public float QuantumResistance { get { return GetSkillLevels()[SkillType.QuantumDefense] * 0.1f; } }
        public float ShieldHeatResistance { get { return GetSkillLevels()[SkillType.ShieldHeatDefense] * 0.1f; } }
        public float ShieldEnergyResistance { get { return GetSkillLevels()[SkillType.ShieldEnergyDefense] * 0.1f; } }
        public float ShieldKineticResistance { get { return GetSkillLevels()[SkillType.ShieldKineticDefense] * 0.1f; } }
        public float ShieldQuantumResistance { get { return GetSkillLevels()[SkillType.ShieldQuantumDefense] * 0.1f; } }
        public float EnergyShieldHeatResistance { get { return GetSkillLevels()[SkillType.EnergyShieldHeatDefense] * 0.1f; } }
        public float EnergyShieldEnergyResistance { get { return GetSkillLevels()[SkillType.EnergyShieldEnergyDefense] * 0.1f; } }
        public float EnergyShieldKineticResistance { get { return GetSkillLevels()[SkillType.EnergyShieldKineticDefense] * 0.1f; } }
        public float EnergyShieldQuantumResistance { get { return GetSkillLevels()[SkillType.EnergyShieldQuantumDefense] * 0.1f; } }

        public bool HasMasterTrader { get { return GetSkillLevels()[SkillType.MasterTrader] > 0; } }
        public float PriceScale { get { return 1f - GetSkillLevels()[SkillType.Trading]*0.05f; } }

        public int CraftingLevelModifier { get { return -GetSkillLevels()[SkillType.CraftingLevel]*5; } }
        public float CraftingPriceScale { get { return 1f - GetSkillLevels()[SkillType.CraftingPrice] * 0.05f; } }

        public long MaxShipExperience { get { return GetSkillLevels()[SkillType.ExceedTheLimits] > 0 ? Maths.Experience.MaxPlayerExperience2 : Maths.Experience.MaxPlayerExperience; } }
        public int MaxEmery { get { return GetSkillLevels()[SkillType.MoreEmery]; } }
        public int MaxAlly { get { return GetSkillLevels()[SkillType.MoreAlly]; } }
        public float StealthRadar { get { return GetSkillLevels()[SkillType.StealthRadar] * 0.05f; } }
        public float JammingRadar { get { return GetSkillLevels()[SkillType.JammingRadar] * 0.05f; } }
        public int LargeShipExploration { get { return GetSkillLevels()[SkillType.LargeShipExploration]; } }

        public int GetAvailableHangarSlots(GameDatabase.Enums.SizeClass size)
        {
            if (size <= GameDatabase.Enums.SizeClass.Undefined)
                return 0;
            if (size == GameDatabase.Enums.SizeClass.Dominate)
                return AvailableHangarSlots(size);

            return AvailableHangarSlots(size) - AvailableHangarSlots(size + 1);
        }

        private int AvailableHangarSlots(GameDatabase.Enums.SizeClass size)
        {
            switch (size)
            {
                case GameDatabase.Enums.SizeClass.Frigate:
                    return 1 + GetSkillLevels()[SkillType.HangarSlot1];
                case GameDatabase.Enums.SizeClass.Destroyer:
                    return GetSkillLevels()[SkillType.HangarSlot2];
                case GameDatabase.Enums.SizeClass.Cruiser:
                    return GetSkillLevels()[SkillType.HangarSlot3];
                case GameDatabase.Enums.SizeClass.Battleship:
                    return GetSkillLevels()[SkillType.HangarSlot4];
                case GameDatabase.Enums.SizeClass.Titan:
                    return GetSkillLevels()[SkillType.HangarSlot5];
                case GameDatabase.Enums.SizeClass.Dominate:
                    return GetSkillLevels()[SkillType.HangarSlot6];
                default:
                    return 0;
            }
        }

        protected override void OnSessionDataLoaded()
        {
            _skillLevels = null;
            _pointsSpent = 0;
            _experience = _session.Upgrades.PlayerExperience;
        }

        protected override void OnSessionCreated()
        {
        }

        private bool CanAddSkill(Skills.SkillInfo skill)
        {
            return skill.Type.IsCommonSkill() && AvailablePoints > 0 && !_session.Upgrades.HasSkill(skill.Id);
        }

        private Dictionary<SkillType, int> GetSkillLevels()
        {
            if (_skillLevels == null)
            {
                _skillLevels = Enum.GetValues(typeof(SkillType)).OfType<SkillType>().ToDictionary(item => item, item => 0);
                foreach (var id in _session.Upgrades.Skills)
                {
                    var info = _skills[id];
                    if (info.IsEmpty)
                        continue;

                    if (!info.Type.IsCommonSkill())
                        continue;

                    _skillLevels[info.Type] += info.Multilpler;
                    _pointsSpent++;
                }

                _messenger.Broadcast(EventType.PlayerSkillsChanged);
            }

            return _skillLevels;
        }

        private Dictionary<SkillType, int> _skillLevels;
        private Experience _experience = 0;
        private int _pointsSpent = 0;
        private readonly Skills _skills;

        private readonly ISessionData _session;
        private readonly IMessenger _messenger;
    }
}
