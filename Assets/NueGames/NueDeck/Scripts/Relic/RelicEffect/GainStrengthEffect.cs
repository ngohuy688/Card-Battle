using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class GainStrengthEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.GainStrength;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value <= 0) return;

            var targets = GetTargets(parameters, TargetType.Ally);
            foreach (var target in targets)
            {
                if (target && !target.CharacterStats.IsDeath)
                {
                    target.CharacterStats.ApplyStatus(StatusType.Strength, value);
                    if (FxManager)
                        FxManager.PlayFx(target.transform, FxType.Str);
                }
            }
        }
    }
}
