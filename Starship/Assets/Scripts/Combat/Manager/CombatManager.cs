using UnityEngine;
using System.Linq;
using Combat.Ai;
using Combat.Component.Ship;
using Combat.Component.Triggers;
using Combat.Component.Unit.Classification;
using Combat.Domain;
using Combat.Factory;
using Combat.Scene;
using Combat.Unit;
using Combat.Unit.Ship.Effects.Special;
using GameServices;
using GameServices.Player;
using GameStateMachine.States;
using Services.Audio;
using Services.Messenger;
using GameDatabase;
using Gui.Combat;
using Maths;
using Model.Military;
using Services.ObjectPool;
using Services.Reources;
using Zenject;
using Combat.Component.Body;
using Combat.Unit.Object;

namespace Combat.Manager
{
    public class CombatManager : IInitializable, ITickable
    {
        private enum Asteroidtype
        {
            small = 1,
            middle,
            large,
            huge,
        };


        [Inject]
        private CombatManager(
            GameFlow gameFlow,
            IMessenger messenger,
            ISoundPlayer soundPlayer,
            PlayerSkills skills,
            ExitSignal.Trigger exitTrigger)
        {
            _gameFlow = gameFlow;
            _soundPlayer = soundPlayer;
            _playerSkills = skills;
            _exitTrigger = exitTrigger;
            _messenger = messenger;

            _messenger.AddListener(EventType.EscapeKeyPressed, OnEscapeKeyPressed);
            _messenger.AddListener<IShip>(EventType.CombatShipCreated, OnShipCreated);
            _messenger.AddListener<IShip>(EventType.CombatShipDestroyed, OnShipDestroyed);
        }

        [Inject] private readonly IScene _scene;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly IAiManager _aiManager;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ShipControlsPanel _shipControlsPanel;
        [Inject] private readonly ShipFactory _shipFactory;
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly SpaceObjectFactory _spaceObjectFactory;
        [Inject] private readonly EffectFactory _effectFactory;

        [Inject] private readonly ShipSelectionPanel _shipSelectionPanel;
        [Inject] private readonly ShipStatsPanel _playerStatsPanel;
        [Inject] private readonly ShipStatsPanel _enemyStatsPanel;
        [Inject] private readonly CombatMenu _combatMenu;
        [Inject] private readonly Settings _settings;
        [Inject] private readonly RadarPanel _radarPanel;
        [Inject] private readonly RadarMapPanel _radarMapPanel;
        [Inject] private readonly IKeyboard _lKeyboard;
        [Inject] private readonly ICombatModel _combatModel;

        public void Initialize()
        {
            UnityEngine.Debug.Log("OnCombatStarted");

            var random = new System.Random(System.DateTime.Now.Second * System.DateTime.Now.Minute * System.DateTime.Now.Hour);

            //if (_combatData.Rules.PlanetEnabled)
            //{
            //    objectFactory.CreatePlanet(_config.PlanetPrefab, _config.AtmospherePrefab, Position.Random(random), Random.Range(0, 360), Vector2.zero, Random.Range(16, 25));
            //}

            var level = Maths.Distance.ToShipNoLimitedLevel(_motherShip.CurrentStar.Level);
            var powerMultiplier = Experience.LevelToPowerMultiplier(Distance.ToShipLevel(level));

            var AsteroidsPoint = 10 + level / 10;
            //int num_level =50;
            //int max_num = random.Range(num_level * num_level, (num_level + 1) * (num_level + 1));
            if (_combatModel.Rules.AsteroidsEnabled)
            {
                int smallAsteroid = 2;
                int middleAsteroid = 5;
                int largeAsteroid = 10;
                int hugeAsteroid = 20;
                int lastAsteroidsPoint = AsteroidsPoint;
                int smallAsteroidcount = Random.Range(0, lastAsteroidsPoint / smallAsteroid);
                lastAsteroidsPoint -= smallAsteroidcount * smallAsteroid;
                int middleAsteroidcount = Random.Range(0, lastAsteroidsPoint / middleAsteroid);
                lastAsteroidsPoint -= middleAsteroidcount * middleAsteroid;
                int largeAsteroidcount = Random.Range(0, lastAsteroidsPoint / largeAsteroid);
                lastAsteroidsPoint -= largeAsteroidcount * largeAsteroid;
                int hugeAsteroidcount = Random.Range(0, lastAsteroidsPoint / hugeAsteroid);
                lastAsteroidsPoint -= hugeAsteroidcount * hugeAsteroid;

                if (lastAsteroidsPoint >= 2)
                    smallAsteroidcount += lastAsteroidsPoint / smallAsteroid;
                
                SummonAsteroid(Asteroidtype.small, smallAsteroidcount, powerMultiplier);
                SummonAsteroid(Asteroidtype.middle, middleAsteroidcount, powerMultiplier);
                SummonAsteroid(Asteroidtype.large, largeAsteroidcount, powerMultiplier);
                SummonAsteroid(Asteroidtype.huge, hugeAsteroidcount, powerMultiplier);
            

                //for (int i = 0; i < max_num; ++i)
                //{
                //    var size = Random.Range(1f, 5f + num_level * 5f);
                //    var position = _scene.FindFreePlace(80f, UnitSide.Undefined);

                //    var weight = size * size * size * 500f;
                //    var hitPoints = size * size * size * 10000 * powerMultiplier;
                //    var damageMultiplier = powerMultiplier * 100;

                //    var velocity = Random.insideUnitCircle * 100 / size;
                //    _spaceObjectFactory.CreateAsteroid(position, velocity, size, weight, hitPoints, damageMultiplier);
                //}
            }

            if (_combatModel.Rules.PlanetEnabled)
            {
                var r = random.NextFloat();
                var g = Mathf.Sqrt(1f - r * r);
                var b = random.NextFloat();
                var color = Color.Lerp(new Color(r, g, b), Color.gray, 0.5f);

                var position = new Vector2(_scene.Settings.AreaWidth * random.NextFloat(), _scene.Settings.AreaHeight * random.NextFloat());
                var size = 100 + random.NextFloat() * 30;
                _spaceObjectFactory.CreatePlanet(position, size, color);
            }

            if (_combatModel.Rules.InitialEnemies > 1)
                foreach (var ship in _combatModel.EnemyFleet.Ships.Where(item => item.Status == ShipStatus.Ready).Skip(1).Take(_combatModel.Rules.InitialEnemies - 1))
                    CreateShip(ship);

            _radarMapPanel.IsHide = false;
        }

        private void SummonAsteroid(Asteroidtype asteroidtype, int count, float powerMultiplier)
        {
            for (int i = 0; i < count; ++i)
            {
                float size;
                switch(asteroidtype)
                {
                    case Asteroidtype.small:
                        size = Random.Range(0.5f, 3f);
                        break;
                    case Asteroidtype.middle:
                        size = Random.Range(3f, 8f);
                        break;
                    case Asteroidtype.large:
                        size = Random.Range(8f, 15f);
                        break;
                    case Asteroidtype.huge:
                        size = Random.Range(15f, 25f);
                        break;
                    default:
                        size = Random.Range(0.5f, 3f);
                        break;
                }
                var position = _scene.FindFreePlace(size * 2f, UnitSide.Undefined);

                var weight = 10 * size * size * size;
                var hitPoints = 500 * size * size  * powerMultiplier;
                var damageMultiplier = 0.2f * size * powerMultiplier;

                var velocity = Random.insideUnitCircle * 10 / Mathf.Pow(size, 0.75f) * Random.value;
                _spaceObjectFactory.CreateAsteroid(position, velocity, size, weight, hitPoints, damageMultiplier);
            }
        }


        public void OnShipCreated(IShip ship)
        {
            if (ship.Type.Class != UnitClass.Ship && ship.Type.Class != UnitClass.Drone)
                return;

            CheckIfCanCallNextEnemy();
            CheckIfCanCallNextAlly();
            switch (ship.Type.Side)
            {
                case UnitSide.Player:
                    if (ship.Type.Class == UnitClass.Drone)
                    {
                        _radarMapPanel.Add(ship);
                        break;
                    }
                    _shipControlsPanel.Load(ship);
                    _radarMapPanel.Add(ship);
                    _messenger.Broadcast(EventType.PlayerShipCountChanged,
                        _combatModel.PlayerFleet.Ships.Count(item => item.Status == ShipStatus.Ready));
                    break;
                case UnitSide.Enemy:
                    _radarMapPanel.Add(ship);
                    if (ship.Type.Class == UnitClass.Ship)
                    {
                        _radarPanel.Add(ship);
                        _messenger.Broadcast(EventType.EnemyShipCountChanged,
                            _combatModel.EnemyFleet.Ships.Count(item => item.Status == ShipStatus.Ready));
                    }
                    break;
                case UnitSide.Ally:
                    _radarMapPanel.Add(ship);
                    if (ship.Type.Class == UnitClass.Ship)
                    {
                        _radarPanel.Add(ship);
                        _messenger.Broadcast(EventType.PlayerShipCountChanged,
                            _combatModel.PlayerFleet.Ships.Count(item => item.Status == ShipStatus.Ready));
                    }
                    break;
            }
        }

        public void OnShipDestroyed(IShip ship)
        {
            if (ship.Type.Class != UnitClass.Ship)
                return;
            
            CheckIfCanCallNextEnemy();
            CheckIfCanCallNextAlly();
        }

        public void CreateShip(IShipInfo ship)
        {
            var position = _scene.FindFreePlace(80, ship.Side);

            var controllerFactory = ship.Side == UnitSide.Player ? (IControllerFactory)new KeyboardController.Factory(_lKeyboard) 
                : ship.Side == UnitSide.Enemy ? new Computer.Factory(_scene, _combatModel.EnemyFleet.Level) 
                : new Computer.Factory(_scene, _combatModel.PlayerFleet.Level);
            if (ship.Side == UnitSide.Ally)
                ship.Create(_shipFactory, controllerFactory, _scene.PlayerShip.Body.Position + Random.insideUnitCircle * 3 * (ship.ShipData.Model.ModelScale + _scene.PlayerShip.Body.Scale));
            else
                ship.Create(_shipFactory, controllerFactory, position);
            //UnityEngine.Debug.Log("CreateShip.start - " + ship.Name);
            //var context = new FactoryContext(_scene, _bindingManager, _soundPlayer, _objectPool, _resourceLocator, _settings);
            //var shipModel = fleet.ActivateShip(ship, position, Random.Range(0, 360), _gameSettings.ShowDamage, _playerSkills, _messenger, context, _aiManager, _database);
            ////UnityEngine.Debug.Log("CreateShip.end");
            //return shipModel;
        }

        public bool IsGamePaused { get { return _pausedCount > 0; } }

        public void OnEscapeKeyPressed()
        {
            if (_combatMenu)
                _combatMenu.Open();
        }

        public void Surrender()
        {
            _combatModel.PlayerFleet.DestroyAllShips();
            Exit();
        }

        public void Exit()
        {
            _gameFlow.Pause();
            _exitTrigger.Fire();
        }

        public bool CanChangeShip()
        {
            return _combatModel.Rules.CanSelectShips && _playerSkills.HasRescueUnit && _combatModel.PlayerFleet.IsAnyShipLeft();
        }

        public void ChangeShip()
        {
            var player = _scene.PlayerShip;
            if (player.Effects.All.OfType<ShipRetreatEffect>().Any())
                return;

            var chargeEffect = new ShipRetreatingEffect(player, _effectFactory, ConditionType.OnActivate, ConditionType.OnDeactivate);
            var warpEffect = new ShipWarpEffect(player, _effectFactory, _soundPlayer, _settings.ShipWarpSound, ConditionType.OnDeactivate);
            var soundEffect = new SoundLoopEffect(_soundPlayer, _settings.ShipRetreatSound, ConditionType.OnActivate, ConditionType.OnDeactivate);
            player.AddEffect(new ShipRetreatEffect(7.0f, soundEffect, warpEffect, chargeEffect));
        }

        public void KillAllEnemies()
        {
            _combatModel.EnemyFleet.DestroyAllShips();
        }

        public bool CanCallNextEnemy() { return _canCallNextEnemy; }
        public bool CanCallNextAlly() { return _canCallNextAlly; }

        public void CallNextEnemy()
        {
            if (!CanCallNextEnemy())
                return;

            var shipInfo = _combatModel.EnemyFleet.Ships.FirstOrDefault(item => item.Status == ShipStatus.Ready);
            if (shipInfo == null)
                return;

            CreateShip(shipInfo);
            _soundPlayer.Play(_settings.ReinforcementSound);
        }
        public void CallNextAlly()
        {
            if (!CanCallNextAlly())
                return;

            var shipInfo = _combatModel.PlayerFleet.Ships.FirstOrDefault(item => item.Status == ShipStatus.Ready);
            if (shipInfo == null)
                return;
            shipInfo.ChangeSide(UnitSide.Ally);

            CreateShip(shipInfo);
            //_soundPlayer.Play(_settings.ReinforcementSound);
        }

        private void CheckIfCanCallNextEnemy()
        {
            var rules = _combatModel.Rules;
            if (rules.TimeoutBehaviour != TimeoutBehaviour.NextEnemy && rules.TimeoutBehaviour != TimeoutBehaviour.AllEnemiesThenDraw)
            {
                _canCallNextEnemy = false;
                return;
            }

            if (rules.TimeLimit <= 0 && !rules.NoLimitSet)
            {
                _canCallNextEnemy = false;
                return;
            }

            if (!rules.NoLimitSet)
                _canCallNextEnemy = _combatModel.EnemyFleet.Ships.Count(item => item.Status == ShipStatus.Active) < 3 + _playerSkills.MaxEmery && _combatModel.EnemyFleet.IsAnyShipLeft();
            else
                _canCallNextEnemy = _combatModel.EnemyFleet.Ships.Count(item => item.Status == ShipStatus.Active) < rules.NoLimitMaxEnemies && _combatModel.EnemyFleet.IsAnyShipLeft();
        }
        private void CheckIfCanCallNextAlly()
        {
            var rules = _combatModel.Rules;
            if (!rules.NoLimitSet)
                _canCallNextAlly = _combatModel.PlayerFleet.Ships.Count(item => item.Status == ShipStatus.Active) < 1 + _playerSkills.MaxAlly && _combatModel.PlayerFleet.IsAnyShipLeft();
            else
                _canCallNextAlly = _combatModel.PlayerFleet.Ships.Count(item => item.Status == ShipStatus.Active) < 1 + rules.NoLimitMaxAllies && _combatModel.PlayerFleet.IsAnyShipLeft();
        }

        public void Tick()
        {
            if (_combatModel == null)
                return;

            UpdateLocalGame();
        }

        private void UpdateLocalGame()
        {
            var player = _scene.PlayerShip;
            var enemy = _scene.EnemyShip;
            var ally = _scene.AllyShip;

            if (!enemy.IsActive())
            {
                _nextShipCooldown += Time.deltaTime;
                if (_nextShipCooldown > _nextShipMaxCooldown)
                {
                    _nextShipCooldown = 0;

                    var shipInfo = _combatModel.EnemyFleet.Ships.FirstOrDefault(item => item.Status == ShipStatus.Ready);
                    if (shipInfo == null)
                    {
                        UnityEngine.Debug.Log("No more ships");
                        Exit();
                        return;
                    }

                    CreateShip(shipInfo);
                }
            }

            if (!player.IsActive())
            {
                if (!_radarMapPanel.IsHide)
                    _radarMapPanel.Close();
                _nextPlayerShipCooldown += Time.deltaTime;
                if (_nextPlayerShipCooldown > _nextShipMaxCooldown)
                {
                    _nextPlayerShipCooldown = 0;

                    if (!_combatModel.PlayerFleet.IsAnyShipLeft())
                    {
                        UnityEngine.Debug.Log("No more ships");
                        Exit();
                    }
                    else if (_combatModel.Rules.CanSelectShips)
                    {
                        _shipSelectionPanel.Open(_combatModel);
                    }
                    else
                    {
                        var shipInfo = _combatModel.PlayerFleet.AnyAvailableShip();
                        CreateShip(shipInfo);
                    }
                }
            }
            /*
            if (!ally.IsActive() && player.IsActive())
            {
                _nextAllyShipCooldown += Time.deltaTime;
                if (_nextAllyShipCooldown > _nextShipMaxCooldown)
                {
                    _nextAllyShipCooldown = 0;

                    if (!_combatModel.PlayerFleet.IsAnyShipLeft())
                    {
                        UnityEngine.Debug.Log("No more ships");
                        //Exit();
                    }
                    else
                    {
                        var shipInfo = _combatModel.PlayerFleet.AnyAvailableShip();
                        shipInfo.ChangeSide(UnitSide.Ally);
                        CreateShip(shipInfo);
                    }
                }
            }
            */
            if (player.IsActive() && !IsGamePaused)
            {
                if (!_radarMapPanel.IsHide)
                    _radarMapPanel.Open();
                _playerStatsPanel.Open(player);
                _enemyStatsPanel.Open(enemy);
            }
        }

        private bool _canCallNextEnemy;
        private bool _canCallNextAlly;

        private float _nextShipCooldown = _nextShipMaxCooldown;
        private float _nextAllyShipCooldown = 0f;
        private float _nextPlayerShipCooldown = _nextShipMaxCooldown;
        private const float _nextShipMaxCooldown = 3.0f;

        private int _pausedCount;
        private readonly GameFlow _gameFlow;
        private readonly ISoundPlayer _soundPlayer;
        private readonly PlayerSkills _playerSkills;
        private readonly ExitSignal.Trigger _exitTrigger;
        private readonly IMessenger _messenger;
    }
}
