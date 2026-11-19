using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class FirstAttackBonusEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.FirstAttackBonus;

        private static int _lastEncounterId = -1;
        private static bool _hasTriggeredThisCombat;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var currentEncounter = GameManager?.PersistentGameplayData?.CurrentEncounterId ?? 0;
            if (_lastEncounterId != currentEncounter)
            {
                _lastEncounterId = currentEncounter;
                _hasTriggeredThisCombat = false;
            }

            if (!_hasTriggeredThisCombat)
            {
                parameters.Value += EffectValue(parameters);
                _hasTriggeredThisCombat = true;
            }
        }
    }
}
