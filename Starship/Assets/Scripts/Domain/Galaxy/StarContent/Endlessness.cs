﻿using Combat.Component.Unit.Classification;
using Combat.Domain;
using GameDatabase;
using GameServices.Player;
using GameServices.Random;
using GameStateMachine.States;
using Session;
using Zenject;

namespace Galaxy.StarContent
{
    public class Endlessness
    {
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly StarContentChangedSignal.Trigger _starContentChangedTrigger;
        [Inject] private readonly StartBattleSignal.Trigger _startBattleTrigger;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;

        public long GetLastAttackTime(int starId)
        {
            return _session.CommonObjects.GetUseTime(starId);
        }

        public long CooldownTime { get { return System.TimeSpan.TicksPerHour*24; } }

        public Model.Military.IFleet CreateFleet(int starId)
        {
            return Model.Factories.Fleet.Endlessness(_starData.GetLevel(starId), 
                _starData.GetRegion(starId).Faction, starId + _random.Seed, _database);
        }

        public void Attack(int starId)
        {
            if (System.DateTime.UtcNow.Ticks < GetLastAttackTime(starId) + CooldownTime)
                throw new System.InvalidOperationException();

            var level = _starData.GetLevel(starId);
            var firstFleet = new Model.Military.PlayerFleet(_database, _playerFleet);
            var secondFleet = CreateFleet(starId);

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = firstFleet;
            builder.EnemyFleet = secondFleet;
            builder.Rules = Model.Factories.CombatRules.Endlessness();

            _startBattleTrigger.Fire(builder.NoLimitBuild(), result => OnCombatCompleted(starId));

            _session.CommonObjects.SetUseTime(starId, System.DateTime.UtcNow.Ticks);
        }

        private void OnCombatCompleted(int starId)
        {
            _starContentChangedTrigger.Fire(starId);
        }

        public struct Facade
        {
            public Facade(Endlessness endlessness, int starId)
            {
                _endlessness = endlessness;
                _starId = starId;
            }

            public Model.Military.IFleet CreateFleet() { return _endlessness.CreateFleet(_starId); }
            public long LastAttackTime { get { return _endlessness.GetLastAttackTime(_starId); } }
            public long CooldownTime { get { return _endlessness.CooldownTime; } }
            public void Attack() { _endlessness.Attack(_starId); }

            private readonly Endlessness _endlessness;
            private readonly int _starId;
        }
    }
}
