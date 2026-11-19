using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class GainBlockEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.GainBlock;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value <= 0) return;

            var targets = GetTargets(parameters, TargetType.Ally);
            foreach (var target in targets)
            {
                if (target && !target.CharacterStats.IsDeath)
                {
                    target.CharacterStats.ApplyStatus(StatusType.Block, value);
                    if (FxManager)
                        FxManager.PlayFx(target.transform, FxType.Block);
                }
            }
        }
    }
}
