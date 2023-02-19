﻿using Combat.Collision;
using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Unit;
using Combat.Unit.Auxiliary;
using UnityEngine;

namespace Combat.Component.Controller
{ 
    public class RepairBotContoller : IController
    {
        public RepairBotContoller(IShip ship, IAuxiliaryUnit repairBot, float radius, float repairRate, float toward = 0)
        {
            _ship = ship;
            _radius = radius;
            _repairBot = repairBot;
            _repairRate = repairRate;
            _toward = toward;
        }

        public virtual void UpdatePhysics(float elapsedTime)
        {
            if (!_ship.IsActive())
            {
                _repairBot.Destroy();
                return;
            }

            var enabled = _repairBot.Enabled;

            _elapsedTime += elapsedTime;
            var active = enabled && _elapsedTime%3f > 2f;

            var requiredRotation = RotationHelpers.Angle(_ship.Body.WorldPosition() - _repairBot.Body.WorldPosition());
            if (active)
                requiredRotation += Random.Range(-30, 30);

            var requiredPosition = _ship.Body.WorldPosition();
            if (enabled)
                requiredPosition += RotationHelpers.Direction(_elapsedTime * _turnRate + _toward) * _radius;

            _repairBot.MoveTowards(requiredPosition, requiredRotation, _ship.Body.WorldVelocity(), _velocityFactor, _angularVelocityFactor);

            _repairBot.Active = active;
            if (active)
            {
                var impact = new Impact(new GameDatabase.Model.AllDamageData(), 0, 0, new Impulse(), CollisionEffect.None);
                impact.AllDamageData.Repair.SetDamage(_repairRate * elapsedTime);
                _ship.Affect(impact);
            } 

            if (!enabled && _repairBot.Body.WorldPosition().Distance(requiredPosition) < _repairBot.Body.Scale)
                _repairBot.Vanish();
        }

        public void Dispose() { }

        private readonly float _toward;
        private float _elapsedTime;
        private readonly float _radius;
        private readonly float _repairRate;
        private readonly IShip _ship;
        private readonly IAuxiliaryUnit _repairBot;

        private const float _turnRate = 45f;
        private const float _velocityFactor = 0.75f;
        private const float _angularVelocityFactor = 5.0f;
    }
}
