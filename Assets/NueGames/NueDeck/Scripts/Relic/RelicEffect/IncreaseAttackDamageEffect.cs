using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class IncreaseAttackDamageEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.IncreaseAttackDamage;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            parameters.Value += EffectValue(parameters);
        }
    }
}
