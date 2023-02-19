using GameServices.LevelManager;
using Services.Messenger;
using Constructor.Satellites;
using Utils;
using Zenject;

namespace GameStateMachine.States
{
    class ConstructorSatelliteState : BaseState
    {
        [Inject]
        public ConstructorSatelliteState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            IMessenger messenger,
            ISatellite satellite,
            ExitSignal exitSignal,
            SatelliteSelectedSignal satelliteSelectedSignal)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _satellite = satellite;
            _messenger = messenger;
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
            _satelliteSelectedSignal = satelliteSelectedSignal;
            _satelliteSelectedSignal.Event += OnsatelliteSelected;
        }

        public override StateType Type { get { return StateType.Constructor; } }

        protected override LevelName RequiredLevel { get { return LevelName.Constructor; } }

        protected override void OnLevelLoaded()
        {
            OnsatelliteSelected(_satellite);
        }

        private void OnExit()
        {
            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private void OnsatelliteSelected(ISatellite satellite)
        {
            if (IsActive)
            {
                _satellite = satellite;
                _messenger.Broadcast(EventType.ConstructorSatelliteChanged, _satellite);
            }
        }

        private ISatellite _satellite;
        private readonly ExitSignal _exitSignal;
        private readonly SatelliteSelectedSignal _satelliteSelectedSignal;
        private readonly IMessenger _messenger;

        public class Factory : Factory<ISatellite, ConstructorSatelliteState> { }
    }

    public class SatelliteSelectedSignal : SmartWeakSignal<ISatellite>
    {
        public class Trigger : TriggerBase { }
    }
}
