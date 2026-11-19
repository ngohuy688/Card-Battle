using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class DrawCardEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.DrawCard;

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            var value = EffectValue(parameters);
            if (value > 0 && CollectionManager)
                CollectionManager.DrawCards(value);
        }
    }
}
