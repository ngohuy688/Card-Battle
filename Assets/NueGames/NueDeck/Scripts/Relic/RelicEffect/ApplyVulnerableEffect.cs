using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class ApplyVulnerableEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.ApplyVulnerable;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value <= 0) return;

            var targets = GetTargets(parameters, TargetType.AllEnemies);
            foreach (var target in targets)
            {
                if (target && !target.CharacterStats.IsDeath)
                {
                    target.CharacterStats.ApplyStatus(StatusType.Vulnerable, value);
                    if (FxManager)
                        FxManager.PlayFx(target.transform, FxType.Buff);
                }
            }
        }
    }
}
