using Combat.Component.Ship;
using Combat.Component.Systems;
using Combat.Factory;
using Combat.Unit;
using Combat.Unit.Auxiliary;
using GameDatabase.Model;
using UnityEngine;

namespace Combat.Component.Triggers
{
    public class RepairBotAction : IUnitAction
    {
        public RepairBotAction(IShip ship, ISystem device, SatelliteFactory factory, float repairRate, float size, Color color, AudioClipId activationSound, int count = 1)
        {
            _factory = factory;
            _ship = ship;
            _size = size;
            _color = color;
            _repairRate = repairRate;
            _repairBotDevice = device;
            _activationSound = activationSound;
            _count = count;
            if (_count > 1)
                _repairBots = new IAuxiliaryUnit[_count];
        }

        public ConditionType TriggerCondition { get { return ConditionType.OnActivate | ConditionType.OnDeactivate; } }

        public bool TryUpdateAction(float elapsedTime)
        {
            if (_count > 1)
            {
                int num = 0;
                bool re = false;
                for (int i = 0; i < _count; i++)
                    if (_repairBots[i].State == UnitState.Destroyed)
                    {
                        num++;
                        re = re || _repairBots[i].State == UnitState.Active;
                    }
                if(num==_count)
                {
                    _repairBotDevice.Enabled = false;
                    return false;
                }
                return re;
            }
            else
            {
                if (_repairBot.State == UnitState.Destroyed)
                {
                    _repairBotDevice.Enabled = false;
                    return false;
                }
                return _repairBot.State == UnitState.Active;
            }
        }

        public bool TryInvokeAction(ConditionType condition)
        {
            if (_count > 1)
            {
                if (condition.Contains(ConditionType.OnDeactivate))
                {
                    for (int i = 0; i < _count; i++)
                        if (_repairBots[i].IsActive())
                            _repairBots[i].Enabled = false;
                }
                else if (condition.Contains(ConditionType.OnActivate))
                {
                    for (int i = 0; i < _count; i++)
                    {
                        if (_repairBots[i] == null || _repairBots[i].State == UnitState.Inactive)
                            _repairBots[i] = _factory.CreateRepairBot(_ship, _repairRate, _size, _size, 1f, _color, _activationSound, i * 360.0f / _count);
                        if (_repairBots[i].IsActive())
                            _repairBots[i].Enabled = true;
                    }
                    return true;
                }
            }
            else
            {
                if (condition.Contains(ConditionType.OnDeactivate))
                {
                    if (_repairBot.IsActive())
                        _repairBot.Enabled = false;
                }
                else if (condition.Contains(ConditionType.OnActivate))
                {
                    if (_repairBot == null || _repairBot.State == UnitState.Inactive)
                        _repairBot = _factory.CreateRepairBot(_ship, _repairRate, _size, _size, 1f, _color, _activationSound);

                    if (_repairBot.IsActive())
                        _repairBot.Enabled = true;

                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            if (_count > 1)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (_repairBots[i].IsActive())
                        _repairBots[i].Destroy();
                }
            }
            else
            {
                if (_repairBot.IsActive())
                    _repairBot.Destroy();
            }
        }

        private readonly int _count;
        private IAuxiliaryUnit _repairBot;
        private IAuxiliaryUnit[] _repairBots = new IAuxiliaryUnit[0];
        private readonly ISystem _repairBotDevice;
        private readonly float _repairRate;
        private readonly Color _color;
        private readonly AudioClipId _activationSound;
        private readonly float _size;
        private readonly IShip _ship;
        private readonly SatelliteFactory _factory;
    }
}
