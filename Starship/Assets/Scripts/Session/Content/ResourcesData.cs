using System;
using System.Collections.Generic;
using System.Linq;
using DebugLogSetting;
using GameDatabase.DataModel;
using GameModel.Serialization;
using Utils;
using Zenject;

namespace Session.Content
{
    public class ResourcesData : ISerializableData
	{
        [Inject]
        public ResourcesData(
            FuelValueChangedSignal.Trigger fuelValueChangedTrigger, 
            MoneyValueChangedSignal.Trigger moneyValueChangedTrigger, 
            StarsValueChangedSignal.Trigger starsValueChangedTrigger, 
            TokensValueChangedSignal.Trigger tokensValueChangedTrigger,
            ResourcesChangedSignal.Trigger specialResourcesChangedTrigger,
            byte[] buffer = null)
        {
            _moneyValueChangedTrigger = moneyValueChangedTrigger;
            _fuelValueChangedTrigger = fuelValueChangedTrigger;
            _starsValueChangedTrigger = starsValueChangedTrigger;
            _tokensValueChangedTrigger = tokensValueChangedTrigger;
            _specialResourcesChangedTrigger = specialResourcesChangedTrigger;

            IsChanged = true;
			_money = 5000;
            _stars = 20;
            _tokens = 0;
            _fuel = GameServices.Player.MotherShip.FuelMinimum;

			Statsbackup(5000, out _moneybackup);
            Statsbackup(20, out _starsbackup);
            Statsbackup(0, out _tokensbackup);
			Statsbackup(GameServices.Player.MotherShip.FuelMinimum, out _fuelbackup);
			
            if (buffer != null && buffer.Length > 0)
                Deserialize(buffer);

            _resources.CollectionChangedEvent += OnCollectionChanged;
        }

        public string FileName { get { return Name; } }
        public const string Name = "resources";

        public bool IsChanged { get; private set; }
		public static int CurrentVersion { get { return 6; } }

		public int Money 
		{
			get 
			{
				if (_money == Statsgetbackup(_moneybackup))
				{
					return _money;
				}
				else
				{
					return 0;
				}
			}
			set
			{
                if (_money == value)
                    return;
				if (_money == Statsgetbackup(_moneybackup))
				{
					IsChanged = true;
					_money = value;
                    Statsbackup(_money, out _moneybackup);
                    _moneyValueChangedTrigger.Fire(_money);
				}
				else
				{
					_money = 0;
                    //Statsbackup(_money, out _moneybackup);
                }
            }
		}
		
		public int Fuel
		{
            get
            {
                if (_fuel == Statsgetbackup(_fuelbackup))
                {
                    return _fuel;
                }
                else
                {
                    return 0;
                }
            }
			set
			{
				if (_fuel == value)
					return;
				if (_fuel == Statsgetbackup(_fuelbackup))
				{

					IsChanged = true;
					_fuel = value;
					Statsbackup(_fuel, out _fuelbackup);
					_fuelValueChangedTrigger.Fire(_fuel);
				}
                else
                {
                    _fuel = 0;
                    //Statsbackup(_fuel, out _fuelbackup);
                }
            }
        }

        public int Tokens
        {
            get
            {
                if (_tokens == Statsgetbackup(_tokensbackup))
                {
                    return _fuel;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (_tokens == value)
                    return;
                if (_tokens == Statsgetbackup(_tokensbackup))
                {

                    IsChanged = true;
                    _tokens = value;
                    Statsbackup(_tokens, out _tokensbackup);
                    _tokensValueChangedTrigger.Fire(_tokens);
                }
                else
                {
                    _tokens = 0;
                    //Statsbackup(_tokens, out _tokensbackup);
                }
            }
        }

        public int Stars
		{
            get
            {
                if (_stars == Statsgetbackup(_starsbackup))
                {
                    return _stars;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (_stars == value)
                    return;
                if (_stars == Statsgetbackup(_starsbackup))
                {

                    IsChanged = true;
                    _stars = value;
                    Statsbackup(_stars, out _starsbackup);
                    _starsValueChangedTrigger.Fire();
                }
                else
                {
                    _stars = 0;
                    //Statsbackup(_stars, out _starsbackup);
                }
            }
        }

        public IGameItemCollection<int> Resources { get { return _resources; } }

		public IEnumerable<byte> Serialize()
		{
			IsChanged = false;
			
			foreach (var value in BitConverter.GetBytes(CurrentVersion))
				yield return value;

			foreach (var value in Helpers.Serialize(Money))
				yield return value;
			foreach (var value in Helpers.Serialize(Fuel))
				yield return value;
			foreach (var value in Helpers.Serialize(Stars))
				yield return value;
            foreach (var value in Helpers.Serialize(Tokens))
                yield return value;
		    foreach (var value in Helpers.Serialize(_resources))
		        yield return value;

		    foreach (var value in Helpers.Serialize(_moneybackup)) // 备份
		        yield return value;
		    foreach (var value in Helpers.Serialize(_fuelbackup))
		        yield return value;
		    foreach (var value in Helpers.Serialize(_starsbackup))
		        yield return value;
		    foreach (var value in Helpers.Serialize(_tokensbackup))
		        yield return value;

		    foreach (var value in Helpers.Serialize(0)) // reserved
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
		    foreach (var value in Helpers.Serialize(0))
		        yield return value;
        }

		private void Deserialize(byte[] buffer)
		{
			if (buffer == null || buffer.Length == 0)
				throw new ArgumentException();

			int index = 0;
			var version = Helpers.DeserializeInt(buffer, ref index);
			if (version != CurrentVersion && !TryUpgrade(ref buffer, version))
			{
				UnityEngine.Debug.Log("ResourcesData: incorrect data version");
				throw new ArgumentException();
			}

			_money = Helpers.DeserializeInt(buffer, ref index);
			_fuel = Helpers.DeserializeInt(buffer, ref index);
			_stars = Helpers.DeserializeInt(buffer, ref index);
			_tokens = Helpers.DeserializeInt(buffer, ref index);

			_resources.Assign(Helpers.DeserializeDictionary(buffer, ref index));

			_moneybackup = Helpers.DeserializeInt(buffer, ref index);
			_fuelbackup = Helpers.DeserializeInt(buffer, ref index);
			_starsbackup = Helpers.DeserializeInt(buffer, ref index);
			_tokensbackup = Helpers.DeserializeInt(buffer, ref index);

			if (OtherDebugLogSetting.ResourceDebugLog)
			{
				UnityEngine.Debug.Log("ResourcesData: money = " + _money);
				UnityEngine.Debug.Log("ResourcesData: fuel = " + _fuel);
				UnityEngine.Debug.Log("ResourcesData: stars = " + _stars);
				UnityEngine.Debug.Log("ResourcesData: tokens = " + _tokens);
				UnityEngine.Debug.Log("ResourcesData: moneybackup = " + _moneybackup);
				UnityEngine.Debug.Log("ResourcesData: fuelbackup = " + _fuelbackup);
				UnityEngine.Debug.Log("ResourcesData: starsbackup = " + _starsbackup);
				UnityEngine.Debug.Log("ResourcesData: tokensbackup = " + _tokensbackup);
			}

			IsChanged = false;
		}

		private static bool TryUpgrade(ref byte[] data, int version)
		{
			if (version == 1)
			{
				data = Upgrade_1_2(data).ToArray();
				version = 2;
			}

			if (version == 2)
			{
				data = Upgrade_2_3(data).ToArray();
				version = 3;
			}

            if (version == 3)
            {
                data = Upgrade_3_4(data).ToArray();
                version = 4;
            }

		    if (version == 4)
		    {
		        data = Upgrade_4_5(data).ToArray();
		        version = 5;
		    }

			if(version == 5)
            {
                data = Upgrade_5_6(data).ToArray();
                version = 6;
            }

            return version == CurrentVersion;
		}
		
		private static IEnumerable<byte> Upgrade_1_2(byte[] buffer)
		{
			int index = 0;
			
			Helpers.DeserializeInt(buffer, ref index);
			var version = 2;
			foreach (var value in Helpers.Serialize(version))
				yield return value;				

			for (int i = index; i < buffer.Length; ++i)
				yield return buffer[i];

			foreach (var value in Helpers.Serialize(0)) // commonResourcesCount
				yield return value;
		}

		private static IEnumerable<byte> Upgrade_2_3(byte[] buffer)
		{
			int index = 0;

			Helpers.DeserializeInt(buffer, ref index);
			var version = 3;
			foreach (var value in Helpers.Serialize(version))
				yield return value;				

			var money = Helpers.DeserializeInt(buffer, ref index);
			var fuel = Helpers.DeserializeInt(buffer, ref index);
			var stars = Helpers.DeserializeInt(buffer, ref index);

			var count = Helpers.DeserializeInt(buffer, ref index);
			for (int i = 0; i < count; ++i)
			{
				var key = Helpers.DeserializeInt(buffer, ref index);
				var value = Helpers.DeserializeInt(buffer, ref index);
				stars += (value+4)/5;
			}

			foreach (var value in Helpers.Serialize(money))
				yield return value;				
			foreach (var value in Helpers.Serialize(fuel))
				yield return value;
			foreach (var value in Helpers.Serialize(stars))
				yield return value;				

			for (int i = index; i < buffer.Length; ++i)
				yield return buffer[i];
		}

        private static IEnumerable<byte> Upgrade_3_4(byte[] buffer)
        {
            int index = 0;

            Helpers.DeserializeInt(buffer, ref index); // version
            foreach (var value in Helpers.Serialize(4))
                yield return value;

            var money = Helpers.DeserializeInt(buffer, ref index);
            var fuel = Helpers.DeserializeInt(buffer, ref index);
            var stars = Helpers.DeserializeInt(buffer, ref index);

            foreach (var value in Helpers.Serialize(money))
                yield return value;
            foreach (var value in Helpers.Serialize(fuel))
                yield return value;
            foreach (var value in Helpers.Serialize(stars))
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // tokens
                yield return value;
            foreach (var value in Helpers.Serialize(0)) // reserved
                yield return value;

            for (int i = index; i < buffer.Length; ++i)
                yield return buffer[i];
        }

	    private static IEnumerable<byte> Upgrade_4_5(byte[] buffer)
	    {
	        int index = 0;

	        Helpers.DeserializeInt(buffer, ref index); // version
	        foreach (var value in Helpers.Serialize(5))
	            yield return value;

	        var money = Helpers.DeserializeInt(buffer, ref index);
	        var fuel = Helpers.DeserializeInt(buffer, ref index);
	        var stars = Helpers.DeserializeInt(buffer, ref index);
	        var tokens = Helpers.DeserializeInt(buffer, ref index);


	        foreach (var value in Helpers.Serialize(money))
	            yield return value;
	        foreach (var value in Helpers.Serialize(fuel))
	            yield return value;
	        foreach (var value in Helpers.Serialize(stars))
	            yield return value;
	        foreach (var value in Helpers.Serialize(tokens))
	            yield return value;

	        var resources = Helpers.DeserializeDictionary(buffer, ref index);
	        var commonResources = Helpers.DeserializeDictionary(buffer, ref index);

	        foreach (var item in commonResources)
	        {
	            int value;
                if (resources.TryGetValue(item.Key, out value))
                    resources[item.Key] = value + item.Value;
                else
                    resources.Add(item.Key, item.Value);
	        }

	        foreach (var value in Helpers.Serialize(resources))
	            yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
        }
	    private static IEnumerable<byte> Upgrade_5_6(byte[] buffer)
	    {
	        int index = 0;

	        Helpers.DeserializeInt(buffer, ref index); // version
	        foreach (var value in Helpers.Serialize(6))
	            yield return value;

	        var money = Helpers.DeserializeInt(buffer, ref index);
	        var fuel = Helpers.DeserializeInt(buffer, ref index);
	        var stars = Helpers.DeserializeInt(buffer, ref index);
	        var tokens = Helpers.DeserializeInt(buffer, ref index);


	        foreach (var value in Helpers.Serialize(money))
	            yield return value;
	        foreach (var value in Helpers.Serialize(fuel))
	            yield return value;
	        foreach (var value in Helpers.Serialize(stars))
	            yield return value;
	        foreach (var value in Helpers.Serialize(tokens))
	            yield return value;

			var resources = Helpers.DeserializeDictionary(buffer, ref index);
            foreach (var value in Helpers.Serialize(resources))
	            yield return value;

            Statsbackup(money, out var _moneybackup);
            Statsbackup(fuel, out var _fuelbackup);
            Statsbackup(stars, out var _starsbackup);
            Statsbackup(tokens, out var _tokensbackup);
            foreach (var value in Helpers.Serialize(_moneybackup)) // 备份
                yield return value;
            foreach (var value in Helpers.Serialize(_fuelbackup))
                yield return value;
            foreach (var value in Helpers.Serialize(_starsbackup))
                yield return value;
            foreach (var value in Helpers.Serialize(_tokensbackup))
                yield return value;

            foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
	        foreach (var value in Helpers.Serialize(0)) // reserved
	            yield return value;
        }

        private void OnCollectionChanged()
	    {
	        IsChanged = true;
	        _specialResourcesChangedTrigger.Fire();
	    }

		private static void Statsbackup(int stats,out int backupstats)
		{
			backupstats = stats ^ 0x12345678;
        }
		private static int Statsgetbackup(int backupstats)
		{
			return backupstats ^ 0x12345678;
        }

        private ObscuredInt _money;
		private ObscuredInt _fuel;
		private ObscuredInt _stars;
        private ObscuredInt _tokens;
		private readonly Dictionary<int, int> _commonResources = new Dictionary<int, int>();
	    private readonly GameItemCollection<int> _resources = new GameItemCollection<int>();

        private readonly FuelValueChangedSignal.Trigger _fuelValueChangedTrigger;
        private readonly MoneyValueChangedSignal.Trigger _moneyValueChangedTrigger;
        private readonly StarsValueChangedSignal.Trigger _starsValueChangedTrigger;
        private readonly TokensValueChangedSignal.Trigger _tokensValueChangedTrigger;
	    private readonly ResourcesChangedSignal.Trigger _specialResourcesChangedTrigger;

        private static readonly int _mask = new System.Random((int)DateTime.Now.Ticks).Next();

		private int _moneybackup;
		private int _fuelbackup;
		private int _starsbackup;
		private int _tokensbackup;
	}

    public class MoneyValueChangedSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
    public class FuelValueChangedSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
    public class StarsValueChangedSignal : SmartWeakSignal { public class Trigger : TriggerBase { } }
    public class TokensValueChangedSignal : SmartWeakSignal<int> { public class Trigger : TriggerBase { } }
    public class ResourcesChangedSignal : SmartWeakSignal { public class Trigger : TriggerBase { } }
}
