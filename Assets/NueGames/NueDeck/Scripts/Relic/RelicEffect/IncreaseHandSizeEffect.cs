using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class IncreaseHandSizeEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.IncreaseHandSize;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            // Passive bonus queried by CollectionManager
        }

        public static int GetHandSizeBonus()
        {
            var gameManager = GameManager.Instance;
            var ownedRelics = gameManager?.PersistentGameplayData?.CurrentRelicsList;
            if (ownedRelics == null) return 0;

            var bonus = 0;
            foreach (var relic in ownedRelics)
            {
                if (relic?.RelicEffectDatas == null) continue;
                foreach (var effect in relic.RelicEffectDatas)
                {
                    if (effect != null && effect.EffectType == RelicEffectType.IncreaseHandSize)
                        bonus += effect.EffectValue;
                }
            }

            return bonus;
        }
    }
}
