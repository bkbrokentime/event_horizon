using GameDatabase.Enums;

namespace Model 
{
	//public enum DamageType
	//{
	//	Impact,
	//	Energy,
	//	Heat,
	//	Direct,
	//}

	public static class DamageTypeExtension
	{
		public static string Name(this DamageType type)
		{
			switch (type)
			{
			case DamageType.Impact:
				return "$ImpactDamage";
			case DamageType.Energy:
				return "$EnergyDamage";
			case DamageType.Heat:
				return "$HeatDamage";
			case DamageType.Flame:
				return "$FlameDamage";
			case DamageType.Antimatter:
				return "$AntimatterDamage";
			case DamageType.Corrosion:
				return "$CorrosionDamage";
			case DamageType.Quantum:
				return "$QuantumDamage";
			case DamageType.Darkmatter:
				return "$DarkmatterDamage";
			case DamageType.Darkenergy:
				return "$DarkenergyDamage";
			case DamageType.Annihilation:
				return "$AnnihilationDamage";
			default:
				return "$DirectDamage";
			}
		}
		public static string Description(this DamageType type)
		{
			switch (type)
			{
			case DamageType.Impact:
				return "$ImpactDamageDescription";
			case DamageType.Energy:
				return "$EnergyDamageDescription";
			case DamageType.Heat:
				return "$HeatDamageDescription";
			case DamageType.Flame:
				return "$FlameDamageDescription";
			case DamageType.Antimatter:
				return "$AntimatterDamageDescription";
			case DamageType.Corrosion:
				return "$CorrosionDamageDescription";
			case DamageType.Quantum:
				return "$QuantumDamageDescription";
			case DamageType.Darkmatter:
				return "$DarkmatterDamageDescription";
			case DamageType.Darkenergy:
				return "$DarkenergyDamageDescription";
			case DamageType.Annihilation:
				return "$AnnihilationDamageDescription";
			default:
				return "$DirectDamageDescription";
			}
		}
	}
}
