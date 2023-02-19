using Constructor.Model;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Localization;
using UnityEngine;

namespace Constructor.Modification
{
	class RepairRate : IModification
	{
		public RepairRate(ModificationQuality quality)
		{
			_multiplier = quality.PowerMultiplier(0.2f, 0.4f, 0.6f, 0.7f, 0.8f, 1.1f, 1.25f, 1.5f, 1.75f, 2.0f);
			Quality = quality;
		}

		public string GetDescription(ILocalization localization) { return localization.GetString("$RepairRateMod", Maths.Format.SignedPercent(_multiplier - 1.0f)); }

		public ModificationQuality Quality { get; private set; }

		public void Apply(ref ShipEquipmentStats stats) { }
		public void Apply(ref GameDatabase.DataModel.Weapon weapon) { }

	    public void Apply(ref DeviceStats device)
	    {
	        if (device.DeviceClass == DeviceClass.RepairBot)
	            device.Power *= _multiplier;
	    }

        public void Apply(ref WeaponStats weapon, ref AmmunitionObsoleteStats ammunition) { }
	    public void Apply(ref WeaponStatModifier statModifier) { }
        public void Apply(ref DroneBayStats droneBay) { }

		private readonly float _multiplier;
	}
}
