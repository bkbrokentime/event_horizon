using System.Collections.Generic;
using System.ComponentModel;
using Constructor.Ships.Modification;
using GameDatabase;
using Zenject;

public class ModificationFactory
{
    [Inject]
    public ModificationFactory(IDatabase database)
    {
        _database = database;
    }

    public IShipModification Create(ModificationType type, int seed = -1)
    {
        if (seed < 0)
            seed = new System.Random().Next();

        switch (type)
        {
            case ModificationType.Empty:
                return new EmptyModification();
            case ModificationType.AutoTargeting:
                return new AutoTargetingModification();
            case ModificationType.HeatDefense:
                return new HeatDefenseModification(seed);
            case ModificationType.EnergyDefense:
                return new EnergyDefenseModification(seed);
            case ModificationType.KineticDefense:
                return new KineticDefenseModification(seed);
            case ModificationType.QuantumDefense:
                return new QuantumDefenseModification(seed);

            case ModificationType.ShieldHeatDefense:
                return new ShieldHeatDefenseModification(seed);
            case ModificationType.ShieldEnergyDefense:
                return new ShieldEnergyDefenseModification(seed);
            case ModificationType.ShieldKineticDefense:
                return new ShieldKineticDefenseModification(seed);
            case ModificationType.ShieldQuantumDefense:
                return new ShieldQuantumDefenseModification(seed);

            case ModificationType.EnergyShieldHeatDefense:
                return new EnergyShieldHeatDefenseModification(seed);
            case ModificationType.EnergyShieldEnergyDefense:
                return new EnergyShieldEnergyDefenseModification(seed);
            case ModificationType.EnergyShieldKineticDefense:
                return new EnergyShieldKineticDefenseModification(seed);
            case ModificationType.EnergyShieldQuantumDefense:
                return new EnergyShieldQuantumDefenseModification(seed);

            case ModificationType.ExtraBlueCells:
                return new ExtraCellsModification(seed);
            case ModificationType.Infected:
                return new InfectedModification(seed, _database);
            case ModificationType.LightWeight:
                return new LightWeightModification(seed);
            default:
                throw new InvalidEnumArgumentException();
        }
    }

    public IEnumerable<IShipModification> AvailableMods
    {
        get
        {
            yield return Create(ModificationType.Empty);
            yield return Create(ModificationType.AutoTargeting);
            yield return Create(ModificationType.HeatDefense);
            yield return Create(ModificationType.KineticDefense);
            yield return Create(ModificationType.EnergyDefense);
            yield return Create(ModificationType.QuantumDefense);
            yield return Create(ModificationType.ShieldHeatDefense);
            yield return Create(ModificationType.ShieldKineticDefense);
            yield return Create(ModificationType.ShieldEnergyDefense);
            yield return Create(ModificationType.ShieldQuantumDefense);
            yield return Create(ModificationType.EnergyShieldHeatDefense);
            yield return Create(ModificationType.EnergyShieldKineticDefense);
            yield return Create(ModificationType.EnergyShieldEnergyDefense);
            yield return Create(ModificationType.EnergyShieldQuantumDefense);
            yield return Create(ModificationType.LightWeight);
            yield return Create(ModificationType.Infected);
        }
    }

    private readonly IDatabase _database;
}