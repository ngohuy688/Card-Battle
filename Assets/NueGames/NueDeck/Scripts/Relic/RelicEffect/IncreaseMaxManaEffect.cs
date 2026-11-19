using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class IncreaseMaxManaEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.IncreaseMaxMana;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value <= 0) return;

            if (CombatManager)
            {
                CombatManager.IncreaseMaxMana(value);
            }
            else if (GameManager && GameManager.PersistentGameplayData != null)
            {
                GameManager.PersistentGameplayData.MaxMana += value;
                GameManager.PersistentGameplayData.CurrentMana += value;
                if (UIManager && UIManager.InformationCanvas)
                    UIManager.InformationCanvas.ResetCanvas();
            }
        }
    }
}
