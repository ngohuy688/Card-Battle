using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class TriggerEffectTwiceEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.TriggerEffectTwice;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var times = EffectValue(parameters);
            if (times <= 0 || GameManager == null || GameManager.PersistentGameplayData == null)
                return;

            var ownedRelics = GameManager.PersistentGameplayData.CurrentRelicsList;
            if (ownedRelics == null) return;

            for (var t = 0; t < times; t++)
            {
                foreach (var relic in ownedRelics)
                {
                    if (relic == null || relic == parameters.RelicData) continue;
                    RelicEffectProcessor.Trigger(RelicCondition.OnRelicEquipped, new RelicEffectParameters { RelicData = relic });
                }
            }
        }
    }
}
