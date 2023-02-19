using GameDatabase.Enums;

namespace Model
{
	namespace Military
	{
		public enum RewardType
		{
			Default,
			SpecialOnly,
		}

		//public enum CombatType
		//{
		//	Default,
		//	Training,
		//}

		public enum TimeoutBehaviour
		{
			Decay,
			NextEnemy,
			NextAlly,
            AllEnemiesThenDraw,
            AllAlliesThenDraw,
		}

		public struct CombatRules
		{
			public int TimeLimit;
			public RewardType RewardType;
			//public CombatType CombatType;
            public RewardCondition LootCondition;
            public RewardCondition ExpCondition;
            public TimeoutBehaviour TimeoutBehaviour;
			public bool CanSelectShips;
		    public bool CanCallEnemies;
		    public bool CanCallAllies;
			public bool AsteroidsEnabled;
			public bool PlanetEnabled;
			public bool DisableBonusses;
		    public int InitialEnemies;
            public int MaxEnemies;
			public bool NoLimitSet;
            public int NoLimitMaxEnemies;
            public int NoLimitMaxAllies;
        }
    }
}
