using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class ReduceAllCardCostEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.ReduceAllCardCost;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            // Passive cost reduction queried when cards are evaluated
        }

        public static int GetAllCardCostReduction()
        {
            var gameManager = GameManager.Instance;
            var ownedRelics = gameManager?.PersistentGameplayData?.CurrentRelicsList;
            if (ownedRelics == null) return 0;

            var reduction = 0;
            foreach (var relic in ownedRelics)
            {
                if (relic?.RelicEffectDatas == null) continue;
                foreach (var effect in relic.RelicEffectDatas)
                {
                    if (effect != null && effect.EffectType == RelicEffectType.ReduceAllCardCost)
                        reduction += effect.EffectValue;
                }
            }

            return reduction;
        }
    }
}
