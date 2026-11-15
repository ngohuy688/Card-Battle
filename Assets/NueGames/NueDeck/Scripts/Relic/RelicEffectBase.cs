using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;

namespace NueGames.NueDeck.Scripts.Relic
{
    public class RelicEffectParameters
    {
        public RelicData RelicData { get; internal set; }
        public float Value { get; set; }
        public CharacterBase SourceCharacter { get; set; }
        public CharacterBase TargetCharacter { get; set; }
        public CardData CardData { get; set; }
    }
    public abstract class RelicEffectBase
    {
        protected RelicEffectBase(){}
        public abstract RelicEffectType EffectType { get;}
        public abstract void ApplyEffect(RelicEffectParameters parameters);

        protected int EffectValue(RelicEffectParameters parameters) => parameters.RelicData.EffectValue;
        protected FxManager FxManager => FxManager.Instance;
        protected AudioManager AudioManager => AudioManager.Instance;
        protected GameManager GameManager => GameManager.Instance;
        protected CombatManager CombatManager => CombatManager.Instance;
        protected CollectionManager CollectionManager => CollectionManager.Instance;
    }
}