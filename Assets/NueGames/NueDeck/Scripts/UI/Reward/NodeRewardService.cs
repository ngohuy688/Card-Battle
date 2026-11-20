using NueGames.NueDeck.Scripts.Data.Containers;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.Relic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.UI.Reward
{
    public static class NodeRewardService
    {
        public static bool Apply(NodeRewardEffect effect)
        {
            if (effect == null || !GameManager.Instance || GameManager.Instance.PersistentGameplayData == null)
                return false;

            var run = GameManager.Instance.PersistentGameplayData;
            switch (effect.EffectType)
            {
                case NodeRewardEffectType.GainGold:
                    return GainGold(effect.Amount);
                case NodeRewardEffectType.HealAllAllies:
                    return ApplyHealth(effect.Amount, true);
                case NodeRewardEffectType.DamageAllAllies:
                    return ApplyHealth(effect.Amount, false);
                case NodeRewardEffectType.AddCard:
                    if (!effect.CardData || run.CurrentCardsList == null) return false;
                    run.CurrentCardsList.Add(effect.CardData);
                    return true;
                case NodeRewardEffectType.AddRelic:
                    return effect.RelicData && RelicEffectProcessor.GrantRelic(effect.RelicData);
                default:
                    return false;
            }
        }

        public static bool GainGold(int amount)
        {
            if (amount <= 0 || !GameManager.Instance || GameManager.Instance.PersistentGameplayData == null)
                return false;

            GameManager.Instance.PersistentGameplayData.CurrentGold += amount;
            RelicEffectProcessor.Trigger(RelicCondition.OnCoinGained, new RelicEffectParameters { Value = amount });
            if (UIManager.Instance && UIManager.Instance.InformationCanvas)
                UIManager.Instance.InformationCanvas.SetGoldText(GameManager.Instance.PersistentGameplayData.CurrentGold);
            return true;
        }

        private static bool ApplyHealth(int amount, bool heal)
        {
            if (amount <= 0) return false;
            var run = GameManager.Instance.PersistentGameplayData;
            var allies = run.AllyList;
            if (allies == null) return false;

            var applied = false;
            foreach (var ally in allies)
            {
                if (!ally || !ally.AllyCharacterData) continue;
                var id = ally.AllyCharacterData.CharacterID;
                var health = run.AllyHealthDataList.Find(item => item.CharacterId == id);
                if (health == null)
                {
                    run.SetAllyHealthData(id, ally.AllyCharacterData.MaxHealth, ally.AllyCharacterData.MaxHealth);
                    health = run.AllyHealthDataList.Find(item => item.CharacterId == id);
                }
                if (health == null) continue;

                health.CurrentHealth = heal
                    ? Mathf.Min(health.MaxHealth, health.CurrentHealth + amount)
                    : Mathf.Max(1, health.CurrentHealth - amount);
                applied = true;
            }

            if (applied && UIManager.Instance && UIManager.Instance.InformationCanvas)
                UIManager.Instance.InformationCanvas.ResetCanvas();
            return applied;
        }
    }
}
