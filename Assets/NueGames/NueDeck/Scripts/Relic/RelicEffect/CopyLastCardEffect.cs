using NueGames.NueDeck.Scripts.Enums;

namespace NueGames.NueDeck.Scripts.Relic.RelicEffect
{
    public class CopyLastCardEffect : RelicEffectBase
    {
        public override RelicEffectType EffectType => RelicEffectType.CopyLastCard;

        public static int RepeatCount { get; private set; }

        public override void ApplyEffect(RelicEffectParameters parameters)
        {
            RepeatCount = EffectValue(parameters);
        }

        public static void ConsumeRepeat()
        {
            if (RepeatCount > 0)
                RepeatCount--;
        }
    }
}
