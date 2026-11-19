using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class GainGoldEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.gainGold;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value <= 0 || GameManager == null || GameManager.PersistentGameplayData == null)
                return;

            GameManager.PersistentGameplayData.CurrentGold += value;
            if (UIManager && UIManager.InformationCanvas)
                UIManager.InformationCanvas.SetGoldText(GameManager.PersistentGameplayData.CurrentGold);
        }
    }
}
