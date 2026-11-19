using System.Linq;
using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class ShopDiscountEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.ShopDiscount;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            // Passive modifier queried by ShopCanvas
        }

        public static int GetDiscountedCost(int baseCost)
        {
            var gameManager = NueGames.NueDeck.Scripts.Managers.GameManager.Instance;
            var ownedRelics = gameManager?.PersistentGameplayData?.CurrentRelicsList;
            if (ownedRelics == null) return baseCost;

            var totalDiscount = 0;
            foreach (var relic in ownedRelics)
            {
                if (relic?.RelicEffectDatas == null) continue;
                foreach (var effect in relic.RelicEffectDatas)
                {
                    if (effect != null && effect.EffectType == RelicEffectType.ShopDiscount)
                        totalDiscount += effect.EffectValue;
                }
            }

            if (totalDiscount <= 0) return baseCost;
            return Mathf.Max(0, Mathf.RoundToInt(baseCost * (1f - totalDiscount / 100f)));
        }
    }
}
