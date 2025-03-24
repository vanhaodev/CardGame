using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using World.Card;
using Random = UnityEngine.Random;

namespace World.Card
{
    public partial class CardBattle : MonoBehaviour
    {
        private bool CalCrit(int aCritChance, int aCritDamage, ref int aDmg)
        {
            if (Random.Range(0, 10000) < aCritChance)
            {
                aDmg = (int)(aDmg * (1 + (aCritDamage / 10000f)));
                return true;
            }

            return false;
        }

        private bool CalArmorPiercing(int aPier, int aPierDmg, ref int vDef)
        {
            if (Random.Range(0, 10000) < aPier)
            {
                vDef = (int)(vDef *
                             (1 - (aPierDmg / 10000f)));

                return true;
            }

            return false;
        }

        private bool CalDodge(int vDodgeChance, int vDodgeDmg, ref int aDmg)
        {
            if (Random.Range(0, 10000) < vDodgeChance)
            {
                aDmg =
                    (int)(aDmg * (1 - (vDodgeDmg / 10000f)));
                return true;
            }

            return false;
        }

        private void CalDef(int vDef, ref int aDmg)
        {
            const float armorFactor = 100f; // Hệ số điều chỉnh giáp
            float damageReduction = vDef / (vDef + armorFactor);
            aDmg = (int)(aDmg * (1 - damageReduction));
        }
    }
}