using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat.Collision;
using GameDatabase.Model;

public class DamageStorage : MonoBehaviour
{
    public AllDamageData AllDamageData;
    public int Flame;

    public int Corrosion;

    public void ALLDamageStorage(Impact impact)
    {
        AllDamageData=impact.AllDamageData;
        Flame = impact.Flame;

        Corrosion = impact.Corrosion;
    }

}
