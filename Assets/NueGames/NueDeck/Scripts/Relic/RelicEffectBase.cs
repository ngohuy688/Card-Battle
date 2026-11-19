using System.Collections.Generic;
using NueGames.NueDeck.Scripts.Card;
using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Data.Collection;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.NueExtentions;

namespace NueGames.NueDeck.Scripts.Relic
{
    public class RelicEffectParameters
    {
        public RelicData RelicData { get; internal set; }
        public RelicEffectData CurrentEffectData { get; internal set; }
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

        protected int EffectValue(RelicEffectParameters parameters) =>
            parameters?.CurrentEffectData != null ? parameters.CurrentEffectData.EffectValue : 0;

        protected FxManager FxManager => FxManager.Instance;
        protected AudioManager AudioManager => AudioManager.Instance;
        protected GameManager GameManager => GameManager.Instance;
        protected CombatManager CombatManager => CombatManager.Instance;
        protected CollectionManager CollectionManager => CollectionManager.Instance;
        protected UIManager UIManager => UIManager.Instance;

        protected List<CharacterBase> GetTargets(RelicEffectParameters parameters, TargetType fallbackTarget)
        {
            var list = new List<CharacterBase>();
            if (!CombatManager) return list;

            var targetType = parameters?.CurrentEffectData != null
                ? parameters.CurrentEffectData.TargetType
                : fallbackTarget;

            switch (targetType)
            {
                case TargetType.Ally:
                    if (parameters?.TargetCharacter is AllyBase allyTarget)
                        list.Add(allyTarget);
                    else if (CombatManager.CurrentMainAlly)
                        list.Add(CombatManager.CurrentMainAlly);
                    break;

                case TargetType.AllAllies:
                    if (CombatManager.CurrentAlliesList != null)
                        list.AddRange(CombatManager.CurrentAlliesList);
                    break;

                case TargetType.RandomAlly:
                    if (CombatManager.CurrentAlliesList != null && CombatManager.CurrentAlliesList.Count > 0)
                        list.Add(CombatManager.CurrentAlliesList.RandomItem());
                    break;

                case TargetType.Enemy:
                    if (parameters?.TargetCharacter is EnemyBase enemyTarget)
                        list.Add(enemyTarget);
                    else if (CombatManager.CurrentEnemiesList != null && CombatManager.CurrentEnemiesList.Count > 0)
                        list.Add(CombatManager.CurrentEnemiesList[0]);
                    break;

                case TargetType.AllEnemies:
                    if (CombatManager.CurrentEnemiesList != null)
                        list.AddRange(CombatManager.CurrentEnemiesList);
                    break;

                case TargetType.RandomEnemy:
                    if (CombatManager.CurrentEnemiesList != null && CombatManager.CurrentEnemiesList.Count > 0)
                        list.Add(CombatManager.CurrentEnemiesList.RandomItem());
                    break;
            }

            return list;
        }
    }
}