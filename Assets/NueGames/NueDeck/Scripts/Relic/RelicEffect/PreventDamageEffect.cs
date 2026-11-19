using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class PreventDamageEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.PreventDamage;

        private static int _lastEncounterId = -1;
        private static int _remainingBlocks;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var currentEncounter = GameManager?.PersistentGameplayData?.CurrentEncounterId ?? 0;
            if (_lastEncounterId != currentEncounter)
            {
                _lastEncounterId = currentEncounter;
                _remainingBlocks = EffectValue(parameters);
            }

            if (_remainingBlocks > 0)
            {
                _remainingBlocks--;
                parameters.Value = 0;
                if (parameters.TargetCharacter && FxManager)
                    FxManager.PlayFx(parameters.TargetCharacter.transform, FxType.Block);
            }
        }
    }
}
