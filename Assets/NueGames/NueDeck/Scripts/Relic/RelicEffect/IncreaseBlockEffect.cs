using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class IncreaseBlockEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.IncreaseBlock;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var percent = EffectValue(parameters);
            if (percent <= 0) return;

            var bonus = Mathf.RoundToInt(parameters.Value * (percent / 100f));
            if (bonus <= 0) bonus = 1;

            parameters.Value += bonus;

            var target = parameters.TargetCharacter;
            if (target && !target.CharacterStats.IsDeath)
            {
                target.CharacterStats.ApplyStatus(StatusType.Block, bonus);
                if (FxManager)
                    FxManager.PlayFx(target.transform, FxType.Block);
            }
        }
    }
}
