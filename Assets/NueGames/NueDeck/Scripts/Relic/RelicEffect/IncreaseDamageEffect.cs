using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class IncreaseDamageEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.IncreaseDamage;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            parameters.Value += EffectValue(parameters);
        }
    }
}
