using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Constructor;
using GameDatabase.DataModel;
using Maths;

namespace ViewModel
{
    public class ScorePanelViewModel : MonoBehaviour
    {
        public TextFieldViewModel ShipScore;
        public TextFieldViewModel ArmorDefenseScore;
        public TextFieldViewModel ArmorEnduranceScore;
        public TextFieldViewModel ShieldDefenseScore;
        public TextFieldViewModel ShieldEnduranceScore;
        public TextFieldViewModel EnergyShieldDefenseScore;
        public TextFieldViewModel EnergyShieldEnduranceScore;

        public TextFieldViewModel DEFdefensescore;
        public TextFieldViewModel DEFendurancescore;
        public TextFieldViewModel DEFscore;

        public TextFieldViewModel EnergyScore;
        public TextFieldViewModel EnergyEnduranceScore;
        public TextFieldViewModel ENEscore;

        public TextFieldViewModel EngineScore;

        public TextFieldViewModel DevicesScore;

        public TextFieldViewModel DroneBaysScore;
        public TextFieldViewModel UAVplatformsScore;
        public TextFieldViewModel UAVscore;

        public TextFieldViewModel WeaponsDPS;
        public TextFieldViewModel WeaponsMDPS;
        public TextFieldViewModel WeaponsEPS;
        public TextFieldViewModel WeaponsMEPS;
        public TextFieldViewModel WeaponsConcentration;
        public TextFieldViewModel WeaponsDispersion;
        public TextFieldViewModel WeaponsScore;

        public void UpdateStats(Constructor.IShipSpecification spec, ShipSettings setting)
        {
            var stats = spec.Stats;

            var armordefensescore = Power.ArmorDefenseScore(stats);
            var armorendurancescore = Power.ArmorEnduranceScore(stats);
            var shielddefensescore = Power.ShieldDefenseScore(stats);
            var shieldendurancescore = Power.ShieldEnduranceScore(stats);
            var energyshielddefensescore = Power.EnergyShieldDefenseScore(stats);
            var energyshieldendurancescore = Power.EnergyShieldEnduranceScore(stats);
            ArmorDefenseScore.Value.text = armordefensescore.ToString("N3");
            ArmorEnduranceScore.Value.text = armorendurancescore.ToString("N3");
            ShieldDefenseScore.Value.text = shielddefensescore.ToString("N3");
            ShieldEnduranceScore.Value.text = shieldendurancescore.ToString("N3");
            EnergyShieldDefenseScore.Value.text = energyshielddefensescore.ToString("N3");
            EnergyShieldEnduranceScore.Value.text = energyshieldendurancescore.ToString("N3");
            var defscore = armordefensescore * 2 + shielddefensescore + energyshielddefensescore;
            var defendurancescore = armorendurancescore * 2 + shieldendurancescore + energyshieldendurancescore;
            DEFdefensescore.Value.text = defscore.ToString("N3");
            DEFendurancescore.Value.text = defendurancescore.ToString("N3");
            DEFscore.Value.text = (defscore + defendurancescore).ToString("N3");

            var energyscore = Power.EnergyScore(stats);
            var energyendurancescore = Power.EnergyEnduranceScore(stats);
            EnergyScore.Value.text = energyscore.ToString("N3");
            EnergyEnduranceScore.Value.text = energyendurancescore.ToString("N3");
            var enescore = energyscore + energyendurancescore;
            ENEscore.Value.text = enescore.ToString("N3");

            var enginescore = Power.EngineScore(stats);
            var engscore = enginescore;
            EngineScore.Value.text = enginescore.ToString("N3");

            var devicesscore = Power.DevicesScore(spec);
            var devscore = devicesscore;
            DevicesScore.Value.text = devicesscore.ToString("N3");

            //var dronesscore = Power.DronesScore(spec, setting);
            var dronebaysscore = Power.DroneBaysScore(spec, setting);
            var uavplatformsscore = Power.UAVplatformsScore(spec, setting);
            DroneBaysScore.Value.text = dronebaysscore.ToString("N3");
            UAVplatformsScore.Value.text = uavplatformsscore.ToString("N3");
            var uavscore = dronebaysscore + uavplatformsscore;
            UAVscore.Value.text = uavscore.ToString("N3");

            ArmorDefenseScore.Value.text = Power.ArmorDefenseScore(spec.Stats).ToString("N3");

            var weaponDPS = Power.WeaponsDPS(spec, false);
            var weaponMDPS = Power.WeaponsDPS(spec, true);
            var weaponEPS = Power.WeaponsEPS(spec, false);
            var weaponMEPS = Power.WeaponsEPS(spec, true);
            var weaponconcentration = Power.WeaponsConcentration(spec);
            var weapondispersion = Power.WeaponsDispersion(spec);

            WeaponsDPS.Value.text = weaponDPS.ToString("N3");
            WeaponsMDPS.Value.text = weaponMDPS.ToString("N3");
            WeaponsEPS.Value.text = weaponEPS.ToString("N3");
            WeaponsMEPS.Value.text = weaponMEPS.ToString("N3");
            WeaponsConcentration.Value.text = weaponconcentration.ToString("N3");
            WeaponsDispersion.Value.text = weapondispersion.ToString("N3");
            var weaponsscore = Power.WeaponsScore(spec);
            WeaponsScore.Value.text = weaponsscore.ToString("N3");
            var attscore = weaponsscore * 2;

            var score = defscore + defendurancescore + enescore + engscore + devscore + attscore + uavscore;
            score /= 6;
            ShipScore.Value.text = score.ToString("N3");
        }
    }
}
