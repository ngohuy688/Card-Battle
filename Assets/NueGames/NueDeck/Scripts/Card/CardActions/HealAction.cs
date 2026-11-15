using NueGames.NueDeck.Scripts.Characters;
using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.Relic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class HealAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Heal;

        public override void DoAction(CardActionParameters actionParameters)
        {
            var newTarget = actionParameters.TargetCharacter
                ? actionParameters.TargetCharacter
                : actionParameters.SelfCharacter;

            if (!newTarget) return;
            
            var previousHealth = newTarget.CharacterStats.CurrentHealth;
            newTarget.CharacterStats.Heal(Mathf.RoundToInt(actionParameters.Value));
            var restoredHealth = newTarget.CharacterStats.CurrentHealth - previousHealth;
            if (restoredHealth > 0 && newTarget is AllyBase)
                RelicEffectProcessor.Trigger(RelicCondition.OnHealthRestored,
                    new RelicEffectParameters
                    {
                        Value = restoredHealth,
                        SourceCharacter = actionParameters.SelfCharacter,
                        TargetCharacter = newTarget,
                        CardData = actionParameters.CardData
                    });

            if (FxManager != null) 
                FxManager.PlayFx(newTarget.transform, FxType.Heal);
            
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}