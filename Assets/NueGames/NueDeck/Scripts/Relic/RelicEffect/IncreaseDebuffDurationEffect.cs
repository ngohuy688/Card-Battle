using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class IncreaseDebuffDurationEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.IncreaseDebuffDuration;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            // Passive duration bonus queried when debuffs are applied
        }

        public static int GetDebuffBonus()
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
                    if (effect != null && effect.EffectType == RelicEffectType.IncreaseDebuffDuration)
                        bonus += effect.EffectValue;
                }
            }

            return bonus;
        }
    }
}
