using NueGames.NueDeck.Scripts.Enums;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class HealEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.Heal;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value <= 0) return;

            var targets = GetTargets(parameters, TargetType.Ally);
            if (targets.Count > 0)
            {
                foreach (var target in targets)
                {
                    if (target && !target.CharacterStats.IsDeath)
                    {
                        target.CharacterStats.Heal(value);
                        if (FxManager)
                            FxManager.PlayFx(target.transform, FxType.Heal);
                    }
                }
                return;
            }

            // Outside combat (e.g. OnBattleEnd or OnRelicEquipped)
            if (GameManager && GameManager.PersistentGameplayData != null)
            {
                var allies = GameManager.PersistentGameplayData.AllyList;
                if (allies != null && allies.Count > 0 && allies[0] && allies[0].AllyCharacterData)
                {
                    var id = allies[0].AllyCharacterData.CharacterID;
                    var healthData = GameManager.PersistentGameplayData.AllyHealthDataList?.Find(x => x.CharacterId == id);
                    if (healthData != null)
                    {
                        healthData.CurrentHealth = Mathf.Min(healthData.MaxHealth, healthData.CurrentHealth + value);
                        if (UIManager && UIManager.InformationCanvas)
                            UIManager.InformationCanvas.ResetCanvas();
                    }
                }
            }
        }
    }
}
