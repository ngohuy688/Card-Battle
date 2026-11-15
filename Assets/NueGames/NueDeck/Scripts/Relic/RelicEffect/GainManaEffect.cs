using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class GainManaEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.GainMana;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            if (EffectValue(parameters) > 0 && CombatManager)
                CombatManager.IncreaseMana(EffectValue(parameters));
        }
    }
}
