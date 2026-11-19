using NueGames.NueDeck.Scripts.Characters;
﻿using NueGames.NueDeck.Scripts.Enums;
using NueGames.NueDeck.Scripts.Managers;
using NueGames.NueDeck.Scripts.Relic;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Card.CardActions
{
    public class AttackAction: CardActionBase
    {
        public override CardActionType ActionType => CardActionType.Attack;
        public override void DoAction(CardActionParameters actionParameters)
        {
            if (!actionParameters.TargetCharacter) return;
            
            var targetCharacter = actionParameters.TargetCharacter;
            var selfCharacter = actionParameters.SelfCharacter;
            
            var value = actionParameters.Value + selfCharacter.CharacterStats.StatusDict[StatusType.Strength].StatusValue;

            if (selfCharacter.CharacterStats.StatusDict[StatusType.Weak].StatusValue > 0)
                value = Mathf.RoundToInt(value * 0.75f);

            if (targetCharacter.CharacterStats.StatusDict[StatusType.Vulnerable].StatusValue > 0)
                value = Mathf.RoundToInt(value * 1.5f);
            if (selfCharacter is AllyBase)
            {
                var damageParameters = new RelicEffectParameters
                {
                    Value = value,
                    SourceCharacter = selfCharacter,
                    TargetCharacter = targetCharacter,
                    CardData = actionParameters.CardData
                };
                RelicEffectProcessor.Trigger(RelicCondition.OnDamageDealt, damageParameters);
                value = damageParameters.Value;
            }
            
            targetCharacter.CharacterStats.Damage(Mathf.RoundToInt(value));
            if (targetCharacter is AllyBase)
                RelicEffectProcessor.Trigger(RelicCondition.OnDamageTaken,
                    new RelicEffectParameters
                    {
                        Value = value,
                        SourceCharacter = selfCharacter,
                        TargetCharacter = targetCharacter,
                        CardData = actionParameters.CardData
                    });

            if (FxManager != null)
            {
                FxManager.PlayFx(actionParameters.TargetCharacter.transform,FxType.Attack);
                FxManager.SpawnFloatingText(actionParameters.TargetCharacter.TextSpawnRoot,value.ToString());
            }
           
            if (AudioManager != null) 
                AudioManager.PlayOneShot(actionParameters.CardData.AudioType);
        }
    }
}