using Combat.Component.Ship;

namespace Combat.Ai
{
	public struct ShipControls
	{
		public void Apply(IShip ship)
		{
			ship.Controls.Throttle = _thrust;
			ship.Controls.Course = _course;
		    ship.Controls.SystemsState = _systems;
		}

		public float Course
		{
			set
			{
				if (!_courseLocked)
				{
					_course = value;
					_courseLocked = true;
				}
			} 
		}

		public float Thrust
		{
			set
			{
				if (!_thrustLocked)
				{
					_thrust = value;
					_thrustLocked = true;
				}
			}
			get
			{
				return _thrust;
			}
		}

		public bool IsSystemLocked(int id)
		{
			return (_systemsMask & (1UL << id)) != 0;
        }

	    public void ActivateSystem(int index, bool active = true)
	    {
            if (IsSystemLocked(index)) return;

	        var value = 1UL << index;

	        if (active)
	            _systems |= value;
	        else
	            _systems &= ~value;

	        _systemsMask |= value;
	    }

		public bool RotationLocked { get { return _courseLocked; } }
		public bool MovementLocked { get { return _thrustLocked; } }

		private bool _thrustLocked;
		private float _thrust;
		private bool _courseLocked;
		private float? _course;
	    private ulong _systemsMask;
        private ulong _systems;
    }

    public struct Context
	{
	    public Context(IShip ship, IShip target, IShip ally, TargetList secondaryTargets, ThreatList threats, float currentTime)
	    {
	        Ship = ship;
	        Ally = ally;
	        Enemy = target;
	        Threats = threats;
	        CurrentTime = currentTime;
	        UnusedEnergy = new FloatReference { Value = Ship.Stats.Energy.Value };
	        Targets = secondaryTargets;
	    }

		public float CurrentTime;
		public IShip Ship;
		public IShip Ally;
        public ThreatList Threats;
        public TargetList Targets;
        public IShip Enemy;
	    public FloatReference UnusedEnergy;

        public class FloatReference
        {
            public float Value { get; set; }
        }
	}

	public interface IAction
	{
		void Perform(Context context, ref ShipControls controls);
	}
}
