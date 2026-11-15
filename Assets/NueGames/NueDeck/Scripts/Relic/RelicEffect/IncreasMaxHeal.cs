using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class IncreaseMaxHealthEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.IncreaseMaxHealth;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value <= 0 || GameManager == null)
                return;

            var mainAlly = CombatManager ? CombatManager.CurrentMainAlly : null;
            if (mainAlly)
            {
                mainAlly.CharacterStats.IncreaseMaxHealth(value);
                GameManager.PersistentGameplayData.SetAllyHealthData(
                    mainAlly.AllyCharacterData.CharacterID,
                    mainAlly.CharacterStats.CurrentHealth,
                    mainAlly.CharacterStats.MaxHealth);
                return;
            }

            var allies = GameManager.PersistentGameplayData.AllyList;
            var allyData = allies != null && allies.Count > 0 && allies[0]
                ? allies[0].AllyCharacterData
                : null;
            if (allyData == null)
                return;

            var healthData = GameManager.PersistentGameplayData.AllyHealthDataList.Find(
                x => x.CharacterId == allyData.CharacterID);
            if (healthData == null)
            {
                GameManager.PersistentGameplayData.SetAllyHealthData(
                    allyData.CharacterID,
                    allyData.MaxHealth,
                    allyData.MaxHealth);
                healthData = GameManager.PersistentGameplayData.AllyHealthDataList.Find(
                    x => x.CharacterId == allyData.CharacterID);
            }

            healthData.MaxHealth += value;
        }
    }
}