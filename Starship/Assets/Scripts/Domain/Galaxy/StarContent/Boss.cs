﻿using System;
using Combat.Component.Unit.Classification;
using Combat.Domain;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameModel;
using GameServices.Player;
using GameServices.Random;
using GameStateMachine.States;
using Model.Factories;
using Services.Messenger;
using Session;
using Zenject;

namespace Galaxy.StarContent
{
    public class Boss 
    {
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly ItemTypeFactory _factory;
        [Inject] private readonly StarContentChangedSignal.Trigger _starContentChangedTrigger;
        [Inject] private readonly StartBattleSignal.Trigger _startBattleTrigger;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;


        public bool IsDefeated(int starId)
        {
            return System.DateTime.UtcNow.Ticks < GetDefeatTime(starId) + GetRespawnTime(starId);
        }

        public Model.Military.IFleet CreateFleet(int starId,int level)
        {
            var faction = _starData.GetRegion(starId).Faction;
            if(level<=1)
                return Model.Factories.Fleet.Boss(GetLevel(starId), faction, starId + _random.Seed, _database);
            else
                return Model.Factories.Fleet.Bossesfleet(GetLevel(starId), faction, starId + _random.Seed, _database, level);
        }

        public void Attack(int starId,int level)
        {
            if (IsDefeated(starId))
                throw new InvalidOperationException();

            var faction = _starData.GetRegion(starId).Faction;
            var firstFleet = new Model.Military.PlayerFleet(_database, _playerFleet);

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = firstFleet;
            builder.EnemyFleet = CreateFleet(starId , level);
            builder.Rules = CombatRules.Flagship_rank(GetLevel(starId),3+2*level);
            builder.AddSpecialReward(new Product(_factory.CreateResearchItem(faction)));

            _startBattleTrigger.Fire(builder.Build(), model => OnCombatCompleted(starId, model));
        }

        private void OnCombatCompleted(int starId, ICombatModel combatModel)
        {
            if (combatModel.GetWinner() != UnitSide.Player)
                return;

            _session.Bosses.SetCompleted(starId);
            _starContentChangedTrigger.Fire(starId);
            _starData.GetRegion(starId).OnFleetDefeated();
        }

        private int GetLevel(int starId)
        {
            return _starData.GetLevel(starId) + 5*GetDefeatCount(starId);
        }

        private long GetDefeatTime(int starId)
        {
            return _session.Bosses.CompletedTime(starId);
        }

        private long GetRespawnTime(int starId)
        {
            return _random.RandomInt(starId + GetDefeatCount(starId), 24, 72)*System.TimeSpan.TicksPerHour;
        }

        private int GetDefeatCount(int starId)
        {
            return _session.Bosses.DefeatCount(starId);
        }

        public struct Facade
        {
            public Facade(Boss boss, int starId,int level)
            {
                _boss = boss;
                _starId = starId;
                _level = level;
            }

            public Model.Military.IFleet CreateFleet() { return _boss.CreateFleet(_starId, _level); }
            public bool IsDefeated { get { return _boss.IsDefeated(_starId); } }
            public void Attack() { _boss.Attack(_starId,_level); }

            private readonly Boss _boss;
            private readonly int _starId;
            private readonly int _level;
        }
    }
}
