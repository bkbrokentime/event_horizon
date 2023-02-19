using System.Collections.Generic;
using System.Linq;
using Combat.Domain;
using Constructor.Ships;
using Database.Legacy;
using Domain.Player;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using Session;
using GameServices.LevelManager;
using GameServices.Player;
using Model.Military;
using Scripts.GameStateMachine;
using Utils;
using Zenject;

namespace GameStateMachine.States
{
    public class MainMenuState : BaseState
    {
        [Inject]
        public MainMenuState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            ISessionData session,
            IDatabase database,
            StartGameSignal startGameSignal,
            StartQuickBattleSignal startQuickBattleSignal,
            StartEndlessBattleSignal startEndlessBattleSignal,
            ConfigureControlsSignal configureControlsSignal,
            OpenConstructorSignal openConstructorSignal,
    //        OpenSatelliteConstructorSignal openSatelliteConstructorSignal,
            OpenEhopediaSignal openEhopediaSignal,
            CombatModelBuilder.Factory combatModelBuilderFactory,
            MotherShip motherShip,
            DailyReward dailyReward,
            DailyRewardAwailableSignal dailyRewardAwailableSignal,
            ExitSignal exitSignal)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _dailyReward = dailyReward;
            _dailyRewardAwailableSignal = dailyRewardAwailableSignal;
            _dailyRewardAwailableSignal.Event += CheckDailyReward;

            _motherShip = motherShip;
            _session = session;
            _database = database;
            _combatModelBuilderFactory = combatModelBuilderFactory;

            _startGameSignal = startGameSignal;
            _startGameSignal.Event += OnStartGame;
            _startQuickBattleSignal = startQuickBattleSignal;
            _startQuickBattleSignal.Event += OnStartQuickBattle;
            _startEndlessBattleSignal = startEndlessBattleSignal;
            _startEndlessBattleSignal.Event += OnEndlessQuickBattle;
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
            _configureControlsSignal = configureControlsSignal;
            _configureControlsSignal.Event += OnConfigureControls;
            _openConstructorSignal = openConstructorSignal;
            _openConstructorSignal.Event += OnOpenConstructor;
   //         _openSatelliteConstructorSignal = openSatelliteConstructorSignal;
   //         _openSatelliteConstructorSignal.Event += OnOpenSatelliteConstructor;
            _openEhopediaSignal = openEhopediaSignal;
            _openEhopediaSignal.Event += OnOpenEhopedia;
        }

        public override StateType Type { get { return StateType.MainMenu; } }

        protected override LevelName RequiredLevel { get { return LevelName.MainMenu; } }

        protected override void OnActivate()
        {
#if UNITY_STANDALONE
#else
            CheckDailyReward();
#endif
        }

        private void CheckDailyReward()
        {
            if (!IsActive)
                return;

            if (_dailyReward.IsRewardExists())
                StateMachine.LoadAdditionalState(StateFactory.CreateDaylyRewardState());
        }

        private void OnStartGame()
        {
            if (!IsActive)
                throw new BadGameStateException();

            if (_session.Game.GameStartTime == 0)
            {
                _session.Game.GameStartTime = System.DateTime.UtcNow.Ticks;
                _motherShip.ViewMode = ViewMode.StarSystem;
            }

            StateMachine.LoadAdditionalState(StateFactory.CreateStarMapState());
        }

        private void OnStartQuickBattle(bool easyMode, string testShipId)
        {
            if (!IsActive)
                throw new BadGameStateException();

            Model.Military.IFleet firstFleet, secondFleet;

            int shipId;
            ShipBuild testShip = null;
            if (int.TryParse(testShipId.Replace("*", string.Empty), out shipId))
                testShip = _database.GetShipBuild(new ItemId<ShipBuild>(shipId));

            var random = new System.Random();
            var fleet1 = _database.ShipBuildList.RandomUniqueElements(20, random);
            var fleet2 = _database.ShipBuildList.RandomUniqueElements(20, random);

#if UNITY_EDITOR
            if (testShip != null)
#else
            if (_database.IsEditable && testShip != null)
#endif
            {
                var playerFleet = Enumerable.Repeat(testShip, 1).Concat(fleet1);
                var enemyFleet = testShipId.Contains('*') ? Enumerable.Repeat(testShip,1) : Enumerable.Repeat(testShip,1).Concat(fleet2);

                var aiLevel = testShipId.Contains("**") ? -1 : (easyMode ? 0 : 100);

                firstFleet = new Model.Military.TestFleet(_database, playerFleet, 100);
                secondFleet = new Model.Military.TestFleet(_database, enemyFleet, aiLevel);
            }
            else
            {
                var ships = GetUnlockedShips();
                firstFleet = new Model.Military.TestFleet(_database, ships.RandomUniqueElements(20, random).OrderBy(item => random.Next()), easyMode ? 0 : 100);
                secondFleet = new Model.Military.TestFleet(_database, ships.RandomUniqueElements(20, random).OrderBy(item => random.Next()), easyMode ? 0 : 100);
            }

            var rules = Model.Factories.CombatRules.Default();
            rules.LootCondition = RewardCondition.Never;
            rules.ExpCondition = RewardCondition.Never;
            rules.DisableBonusses = true;
            rules.TimeoutBehaviour = TimeoutBehaviour.NextEnemy;
            //rules.TimeLimit = 30;

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = firstFleet;
            builder.EnemyFleet = secondFleet;
            builder.Rules = rules;

            StateMachine.LoadAdditionalState(StateFactory.CreateCombatState(builder.Build(), null));
        }
        private void OnEndlessQuickBattle(bool easyMode, string testShipId)
        {
            if (!IsActive)
                throw new BadGameStateException();

            Model.Military.IFleet firstFleet, secondFleet;

            int shipId;
            ShipBuild testShip = null;
            if (int.TryParse(testShipId.Replace("*", string.Empty), out shipId))
                testShip = _database.GetShipBuild(new ItemId<ShipBuild>(shipId));

            var random = new System.Random();
            var fleet1 = _database.ShipBuildList.RandomUniqueElements(20, random);
            var fleet2 = _database.ShipBuildList.RandomElements(9999, random);
            /*
            var fleet1list = fleet1.ToList();
            fleet1list.Sort((first, second) => first.Ship.Layout.CellCount + first.Ship.SecondLayout.CellCount - second.Ship.Layout.CellCount - second.Ship.SecondLayout.CellCount);
            var fleet2list = fleet2.ToList();
            fleet2list.Sort((first, second) => first.Ship.Layout.CellCount + first.Ship.SecondLayout.CellCount - second.Ship.Layout.CellCount - second.Ship.SecondLayout.CellCount);
            fleet1 = fleet1list;
            fleet2 = fleet2list;
            */
#if UNITY_EDITOR
            if (testShip != null)
#else
            if (_database.IsEditable && testShip != null)
#endif
            {
                var playerFleet = Enumerable.Repeat(testShip, 1).Concat(fleet1);
                var enemyFleet = testShipId.Contains('*') ? Enumerable.Repeat(testShip,1) : Enumerable.Repeat(testShip,1).Concat(fleet2);

                var aiLevel = testShipId.Contains("**") ? -1 : (easyMode ? 0 : 100);

                firstFleet = new Model.Military.TestFleet(_database, playerFleet, 100);
                secondFleet = new Model.Military.TestFleet(_database, enemyFleet, aiLevel);
            }
            else
            {
                var ships = GetUnlockedShips();
                firstFleet = new Model.Military.TestFleet(_database, ships.RandomUniqueElements(20, random).OrderBy(item => random.Next()), easyMode ? 0 : 100);
                secondFleet = new Model.Military.TestFleet(_database, ships.RandomElements(9999, random).OrderBy(item => random.Next()), easyMode ? 0 : 100);
            }

            var rules = Model.Factories.CombatRules.Default();
            rules.LootCondition = RewardCondition.Never;
            rules.ExpCondition = RewardCondition.Never;
            rules.DisableBonusses = true;
            rules.TimeoutBehaviour = TimeoutBehaviour.NextEnemy;
            //rules.TimeLimit = 30;

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = firstFleet;
            builder.EnemyFleet = secondFleet;
            builder.Rules = rules;

            StateMachine.LoadAdditionalState(StateFactory.CreateCombatState(builder.Build(), null));
        }

        private HashSet<ShipBuild> GetUnlockedShips()
        {
            var ships = new HashSet<ShipBuild>();
            {
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f0s1")));
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f1s1")));
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f2s1")));
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f3s1")));
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f4s1")));
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f5s1")));
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f6s1")));
                ships.Add(_database.GetShipBuild(LegacyShipBuildNames.GetId("f7s1")));

                foreach (var id in _session.Statistics.UnlockedShips)
                {
                    var build = _database.ShipBuildList.Available().FirstOrDefault(item => item.Ship.Id == id && item.DifficultyClass == DifficultyClass.Default);
                    if (build != null)
                        ships.Add(build);
                }
            }

            return ships;
        }

        private void OnConfigureControls()
        {
            StateMachine.LoadAdditionalState(StateFactory.CreateTestingState());
        }

        private void OnOpenConstructor(IShip ship)
        {
            if (IsActive)
                StateMachine.LoadAdditionalState(StateFactory.CreateConstructorState(ship));
        }
   /*     private void OnOpenSatelliteConstructor(Constructor.Satellites.ISatellite satellite)
        {
            if (IsActive)
                StateMachine.LoadAdditionalState(StateFactory.CreateSatelliteConstructorState(satellite));
        }*/

        public void OnOpenEhopedia()
        {
            if (IsActive)
                StateMachine.LoadAdditionalState(StateFactory.CreateEchopediaState());
        }

        private void OnExit()
        {
            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private readonly StartGameSignal _startGameSignal;
        private readonly StartQuickBattleSignal _startQuickBattleSignal;
        private readonly StartEndlessBattleSignal _startEndlessBattleSignal;
        private readonly ConfigureControlsSignal _configureControlsSignal;
        private readonly OpenConstructorSignal _openConstructorSignal;
   //     private readonly OpenSatelliteConstructorSignal _openSatelliteConstructorSignal;
        private readonly OpenEhopediaSignal _openEhopediaSignal;
        private readonly ExitSignal _exitSignal;
        private readonly ISessionData _session;
        private readonly MotherShip _motherShip;
        private readonly IDatabase _database;
        private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;
        private readonly DailyReward _dailyReward;
        private readonly DailyRewardAwailableSignal _dailyRewardAwailableSignal;

        public class Factory : Factory<MainMenuState> { }
    }

    public class StartGameSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

    public class StartQuickBattleSignal : SmartWeakSignal<bool, string>
    {
        public class Trigger : TriggerBase { }
    }
    public class StartEndlessBattleSignal : SmartWeakSignal<bool, string>
    {
        public class Trigger : TriggerBase { }
    }

    public class ConfigureControlsSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
