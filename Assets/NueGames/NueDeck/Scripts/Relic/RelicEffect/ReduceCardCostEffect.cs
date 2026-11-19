using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class ReduceCardCostEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.ReduceCardCost;

        public static int NextCardCostReduction { get; private set; }

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            NextCardCostReduction = EffectValue(parameters);
        }

        public static void ConsumeReduction()
        {
            NextCardCostReduction = 0;
        }
    }
}
