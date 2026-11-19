using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class ThornsEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.Thorns;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var percent = EffectValue(parameters);
            if (percent <= 0) return;

            var attacker = parameters.SourceCharacter;
            if (!attacker || attacker.CharacterStats.IsDeath) return;

            var damageTaken = parameters.Value;
            var reflectedDamage = Mathf.Max(1, Mathf.RoundToInt(damageTaken * (percent / 100f)));

            attacker.CharacterStats.Damage(reflectedDamage);

            if (FxManager)
            {
                FxManager.PlayFx(attacker.transform, FxType.Attack);
                FxManager.SpawnFloatingText(attacker.TextSpawnRoot, reflectedDamage.ToString());
            }
        }
    }
}
